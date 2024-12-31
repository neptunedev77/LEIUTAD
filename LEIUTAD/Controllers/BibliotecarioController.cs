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

        public IActionResult GerirEmprestimos()
        {
            var emprestimos = _context.Emprestimo
                .Include(e => e.Leitor) // Inclui os dados do leitor associado
                .Include(e => e.Emprestimo_Livros) // Inclui a relação de livros
                .Select(e => new
                {
                    e.ID_Emp,
                    e.ID_Leitor,
                    NomeLeitor = e.Leitor.Nome, // Nome do leitor
                    NumeroLivros = e.Emprestimo_Livros.Count, // Contagem de livros associados
                    e.Data_Dev,
                    e.Estado // Adiciona o Estado do empréstimo
                })
                .ToList();

            return View(emprestimos);
        }


        [HttpGet]
        public IActionResult DetalhesEmprestimo(int id)
        {
            // Carregar o empréstimo com todos os dados associados
            var emprestimo = _context.Emprestimo
                .Include(e => e.Leitor) // Inclui o leitor associado
                .Include(e => e.Emprestimo_Livros) // Inclui a tabela associativa
                    .ThenInclude(el => el.Livro) // Inclui os livros através da tabela associativa
                .FirstOrDefault(e => e.ID_Emp == id);

            // Verifica se o empréstimo foi encontrado
            if (emprestimo == null)
            {
                return NotFound(); // Retorna 404 se não for encontrado
            }

            // Passar os livros e o número atual de exemplares para a ViewBag
            var livrosInfo = emprestimo.Emprestimo_Livros.Select(el => new
            {
                Titulo = el.Livro?.Titulo ?? "Livro não encontrado",
                ExemplaresAtuais = el.Livro?.N_Exemplares ?? "0" // Número atual de exemplares
            }).ToList();

            ViewBag.LivrosInfo = livrosInfo;

            return PartialView("_DetalhesEmprestimoModal", emprestimo);
        }



        [HttpGet]
        public IActionResult EditarEmprestimo(int id)
        {
            // Busca o empréstimo pelo ID
            var emprestimo = _context.Emprestimo
                .Include(e => e.Leitor) // Inclui o leitor associado
                .FirstOrDefault(e => e.ID_Emp == id);

            if (emprestimo == null)
            {
                TempData["MensagemErro"] = "Empréstimo não encontrado.";
                return RedirectToAction("GerirEmprestimos");
            }

            return View(emprestimo);
        }

        [HttpPost]
        public IActionResult EditarEmprestimo(Emprestimo emprestimo)
        {
            try
            {
                // Obter o empréstimo existente na base de dados, incluindo os livros associados
                var emprestimoExistente = _context.Emprestimo
                    .Include(e => e.Emprestimo_Livros) // Inclui a relação de livros associados
                        .ThenInclude(el => el.Livro)   // Inclui os dados dos livros
                    .FirstOrDefault(e => e.ID_Emp == emprestimo.ID_Emp);

                if (emprestimoExistente == null)
                {
                    TempData["MensagemErro"] = "Empréstimo não encontrado.";
                    return RedirectToAction("GerirEmprestimos");
                }

                // Verificar se o estado foi alterado de "Por Devolver" para "Devolvido"
                if (emprestimoExistente.Estado == "Por Devolver" && emprestimo.Estado == "Devolvido")
                {
                    foreach (var emprestimoLivro in emprestimoExistente.Emprestimo_Livros)
                    {
                        // Atualizar o número de exemplares do livro
                        if (int.TryParse(emprestimoLivro.Livro.N_Exemplares, out int exemplaresAtuais))
                        {
                            emprestimoLivro.Livro.N_Exemplares = (exemplaresAtuais + 1).ToString();
                        }
                    }
                }
                // Verificar se o estado foi alterado de "Devolvido" para "Por Devolver"
                else if (emprestimoExistente.Estado == "Devolvido" && emprestimo.Estado == "Por Devolver")
                {
                    foreach (var emprestimoLivro in emprestimoExistente.Emprestimo_Livros)
                    {
                        // Atualizar o número de exemplares do livro
                        if (int.TryParse(emprestimoLivro.Livro.N_Exemplares, out int exemplaresAtuais) && exemplaresAtuais > 0)
                        {
                            emprestimoLivro.Livro.N_Exemplares = (exemplaresAtuais - 1).ToString();
                        }
                    }
                }

                // Atualizar apenas o estado do empréstimo
                emprestimoExistente.Estado = emprestimo.Estado;

                // Salvar as alterações na base de dados
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Estado do empréstimo atualizado com sucesso.";
                return RedirectToAction("GerirEmprestimos");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar o empréstimo: {ex.Message}";
                return RedirectToAction("GerirEmprestimos");
            }
        }

        [HttpGet]
        public IActionResult HistoricoEmprestimos(string ordemAtual = "maisRecente")
        {
            // Alterna a ordem para o próximo estado
            var novaOrdem = ordemAtual == "maisRecente" ? "maisAntigo" : "maisRecente";

            // Obter todos os empréstimos com leitores associados e livros associados
            var emprestimos = _context.Emprestimo
                .Include(e => e.Leitor) // Inclui o leitor associado
                .Include(e => e.Emprestimo_Livros) // Inclui os livros associados ao empréstimo
                .AsQueryable();

            // Aplicar ordenação pela Data de Requisição
            if (ordemAtual == "maisAntigo")
            {
                emprestimos = emprestimos.OrderBy(e => e.Data_Req);
            }
            else // Padrão: Mais recente
            {
                emprestimos = emprestimos.OrderByDescending(e => e.Data_Req);
            }

            // Passar a nova ordem para a View
            ViewBag.NovaOrdem = novaOrdem;
            ViewBag.OrdenacaoAtual = ordemAtual;

            return View(emprestimos.ToList());
        }


        [HttpGet]
        public IActionResult HistoricoEmprestimosLeitor(int idLeitor)
        {
            try
            {
                // Obter os empréstimos do leitor pelo ID
                var emprestimos = _context.Emprestimo
                    .Include(e => e.Leitor) // Inclui os dados do leitor associado
                    .Include(e => e.Emprestimo_Livros) // Inclui os livros associados
                    .Where(e => e.ID_Leitor == idLeitor) // Filtra pelo leitor
                    .OrderByDescending(e => e.Data_Req) // Ordena do mais recente para o mais antigo
                    .ToList();

                if (!emprestimos.Any())
                {
                    TempData["MensagemErro"] = "Não foram encontrados empréstimos para este leitor.";
                    return RedirectToAction("GerirEmprestimos");
                }

                // Passa o leitor para a ViewBag para exibir no histórico
                ViewBag.LeitorNome = emprestimos.FirstOrDefault()?.Leitor?.Nome ?? "Leitor não encontrado";
                ViewBag.ID_Leitor = idLeitor;

                return View("HistoricoEmprestimosLeitor", emprestimos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar histórico: {ex.Message}";
                return RedirectToAction("GerirEmprestimos");
            }
        }



    }
}
