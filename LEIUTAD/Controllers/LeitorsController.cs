using Microsoft.AspNetCore.Mvc;
using LEIUTAD.Models;
using LEIUTAD.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LEIUTAD.ViewModels;


namespace LEIUTAD.Controllers
{
    public class LeitorController : Controller
    {
        private readonly LEIUTADContext _context;

        public LeitorController(LEIUTADContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AdicionarAoCarrinho([FromBody] JsonElement data)
        {
            // Extrair o valor de ISBN a partir do JsonElement
            if (!data.TryGetProperty("isbn", out var isbnElement))
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "ISBN inválido ou não fornecido."
                });
            }

            var isbn = isbnElement.GetString();

            // Verificar se o utilizador está autenticado como leitor
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Leitor")
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Precisa de fazer login como leitor para adicionar ao carrinho.",
                    linkLogin = Url.Action("Login", "Account")
                });
            }

            // Verificar se o livro existe e se há exemplares disponíveis
            var livro = _context.Livro.FirstOrDefault(l => l.ISBN == isbn);
            if (livro == null || int.Parse(livro.N_Exemplares) <= 0)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Este livro está esgotado e não pode ser requisitado."
                });
            }

            // Obter o carrinho da sessão
            var carrinho = HttpContext.Session.GetObjectFromJson<List<string>>("Carrinho") ?? new List<string>();

            // Limitar a 5 livros
            if (carrinho.Count >= 5)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Atingiu o limite de livros no carrinho."
                });
            }

            // Adicionar ao carrinho, se ainda não estiver presente
            if (!carrinho.Contains(isbn))
            {
                carrinho.Add(isbn);
                HttpContext.Session.SetObjectAsJson("Carrinho", carrinho);

                return Json(new
                {
                    sucesso = true,
                    mensagem = "Livro adicionado ao carrinho com sucesso.",
                    linkVerCarrinho = Url.Action("VerCarrinho", "Leitor")
                });
            }
            else
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "O livro já está no carrinho."
                });
            }
        }




        [HttpPost]
        public IActionResult RemoverDoCarrinho(string isbn)
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<string>>("Carrinho") ?? new List<string>();

            if (carrinho.Remove(isbn))
            {
                HttpContext.Session.SetObjectAsJson("Carrinho", carrinho);
                TempData["MensagemSucesso"] = "Livro removido do carrinho.";
            }
            else
            {
                TempData["MensagemErro"] = "Livro não encontrado no carrinho.";
            }

            return RedirectToAction("VerCarrinho");
        }



        [HttpGet]
        public IActionResult VerCarrinho()
        {
            var carrinho = HttpContext.Session.GetObjectFromJson<List<string>>("Carrinho") ?? new List<string>();
            var livros = _context.Livro.Include(l => l.Autor).Include(l => l.Genero).Where(l => carrinho.Contains(l.ISBN)).ToList();
            return View(livros); 
        }

        [HttpGet]
        public IActionResult Historico()
        {
            // Obter o ID do leitor autenticado
            var leitorId = HttpContext.Session.GetInt32("UserId");

            if (leitorId == null)
            {
                TempData["MensagemErro"] = "O utilizador não está autenticado.";
                return RedirectToAction("Login", "Account");
            }

            // Obter os dados do histórico de empréstimos e mapear para o ViewModel
            var historico = _context.Emprestimo
                .Include(e => e.Emprestimo_Livros)
                .ThenInclude(el => el.Livro)
                .Where(e => e.ID_Leitor == leitorId)
                .Select(e => new HistoricoEmprestimoViewModel
                {
                    IDEmprestimo = e.ID_Emp,
                    DataRequisicao = e.Data_Req,
                    DataDevolucao = e.Data_Dev,
                    Estado = e.Estado,
                    Livros = e.Emprestimo_Livros.Select(el => el.Livro.Titulo).ToList()
                })
                .ToList();

            return View(historico);
        }


    }
}
