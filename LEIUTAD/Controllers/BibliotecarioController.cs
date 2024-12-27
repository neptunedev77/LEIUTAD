using LEIUTAD.Data;
using LEIUTAD.Models;
using LEIUTAD.Filters; // Certifique-se de que este namespace está correto para o filtro
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LEIUTAD.Controllers
{
    [BibliotecarioAuthorize] // Aplica o filtro a todo o controlador
    public class BibliotecarioController : Controller
    {
        private readonly LEIUTADContext _context;

        public BibliotecarioController(LEIUTADContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["ActiveTab"] = "Dashboard";
            return View();
        }

        [HttpGet]
        public IActionResult GerirLivros()
        {
            ViewData["ActiveTab"] = "GerirLivros";

            var livros = _context.Livro.ToList(); // Obtém os livros
            return View(livros);
        }

        [HttpGet]
        public IActionResult AdicionarLivro()
        {
            // Carregar autores para o dropdown
            var autores = _context.Autor
                .Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = a.ID_Autor.ToString(),
                    Text = a.Autor_Nome
                })
                .ToList();

            var generos = _context.Genero
                .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = g.ID_Genero.ToString(),
                    Text = g.Genero
                })
                .ToList();

            // Validar os resultados antes de passar para a ViewBag
            if (!autores.Any())
            {
                throw new Exception("Nenhum autor encontrado.");
            }

            if (!generos.Any())
            {
                throw new Exception("Nenhum género encontrado.");
            }

            // Passar os valores para a ViewBag
            ViewBag.Autores = autores;
            ViewBag.Generos = generos;

            return View();
        }

        [HttpPost]
        public IActionResult AdicionarLivro(Livros livro, IFormFile Imagem)
        {
            try
            {
                if (Imagem == null || Imagem.Length == 0)
                {
                    ModelState.AddModelError("Imagem", "A imagem é obrigatória.");
                    CarregarDropdowns(); // Método para carregar ViewBag.Autores e ViewBag.Generos novamente
                    return View(livro);
                }

                // Salvar a imagem na pasta wwwroot/images/livros
                var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/livros", Imagem.FileName);
                using (var stream = new FileStream(caminhoImagem, FileMode.Create))
                {
                    Imagem.CopyTo(stream);
                }

                // Guardar o nome da imagem no objeto livro
                livro.Imagem = Imagem.FileName;

                // Adicionar o livro à base de dados
                _context.Livro.Add(livro);
                _context.SaveChanges();

                // Mensagem de sucesso
                TempData["MensagemSucesso"] = "Livro adicionado com sucesso!";
                return RedirectToAction("GerirLivros", "Bibliotecario");
            }
            catch (Exception ex)
            {
                // Mensagem de erro
                TempData["MensagemErro"] = "Erro ao adicionar o livro: " + ex.Message;
                return RedirectToAction("GerirLivros", "Bibliotecario");
            }
        }


        private void CarregarDropdowns()
        {
            ViewBag.Autores = _context.Autor.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = a.ID_Autor.ToString(),
                Text = a.Autor_Nome
            }).ToList();

            ViewBag.Generos = _context.Genero.Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = g.ID_Genero.ToString(),
                Text = g.Genero
            }).ToList();
        }

        [HttpPost]
        public IActionResult EliminarLivro(string isbn)
        {
            try
            {
                // Busca o livro pelo ISBN
                var livro = _context.Livro.FirstOrDefault(l => l.ISBN == isbn);

                if (livro == null)
                {
                    TempData["MensagemErro"] = "O livro não foi encontrado.";
                    return RedirectToAction("GerirLivros");
                }

                // Caminho completo para a imagem na pasta
                var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/livros", livro.Imagem);

                // Apagar a imagem, se existir
                if (System.IO.File.Exists(caminhoImagem))
                {
                    System.IO.File.Delete(caminhoImagem);
                }

                // Remover o livro da base de dados
                _context.Livro.Remove(livro);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Livro eliminado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao tentar eliminar o livro: " + ex.Message;
            }

            return RedirectToAction("GerirLivros");
        }
        // Pop up dos detalhes dos livros
        [HttpGet]
        public IActionResult DetalhesLivro(string id)
        {
            // Buscar o livro pelo ISBN
            var livro = _context.Livro
                .Include(l => l.Autor)
                .Include(l => l.Genero)
                .FirstOrDefault(l => l.ISBN == id);

            if (livro == null)
            {
                return NotFound();
            }

            return PartialView("_DetalhesLivroModal", livro);
        }

        [HttpGet]
        public IActionResult EditarLivro(string id)
        {
            // Busca o livro pelo ISBN
            var livro = _context.Livro.FirstOrDefault(l => l.ISBN == id);

            if (livro == null)
            {
                TempData["MensagemErro"] = "Livro não encontrado.";
                return RedirectToAction("GerirLivros");
            }

            // Carregar dropdowns para autores e géneros
            CarregarDropdowns();

            return View(livro);
        }

        [HttpPost]
        public IActionResult EditarLivro(Livros livro, IFormFile novaImagem)
        {
            try
            {
                // Buscar o livro existente na base de dados para obter a imagem antiga
                var livroExistente = _context.Livro.AsNoTracking().FirstOrDefault(l => l.ISBN == livro.ISBN);

                if (livroExistente == null)
                {
                    TempData["MensagemErro"] = "Livro não encontrado.";
                    return RedirectToAction("GerirLivros");
                }

                // Atribuir a imagem antiga ao objeto livro
                livro.Imagem = livroExistente.Imagem;

                // Verifica se uma nova imagem foi carregada
                if (novaImagem != null && novaImagem.Length > 0)
                {
                    // Apaga a imagem antiga, se necessário
                    var caminhoImagemAntiga = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/livros", livro.Imagem);
                    if (!string.IsNullOrEmpty(livro.Imagem) && livro.Imagem != "default.png" && System.IO.File.Exists(caminhoImagemAntiga))
                    {
                        System.IO.File.Delete(caminhoImagemAntiga);
                    }

                    // Salva a nova imagem
                    var caminhoImagemNova = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/livros", novaImagem.FileName);
                    using (var stream = new FileStream(caminhoImagemNova, FileMode.Create))
                    {
                        novaImagem.CopyTo(stream);
                    }

                    // Atualiza o nome da imagem no objeto do livro
                    livro.Imagem = novaImagem.FileName;
                }

                // Garante que o campo Imagem nunca será nulo
                if (string.IsNullOrEmpty(livro.Imagem))
                {
                    livro.Imagem = "default.png";
                }

                // Atualiza o registro do livro no banco de dados
                _context.Livro.Update(livro);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Livro atualizado com sucesso.";
                return RedirectToAction("GerirLivros");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar livro: {ex.Message}";
                return View(livro);
            }
        }



    }
}
