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

    }
}
