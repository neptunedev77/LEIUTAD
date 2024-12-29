using Microsoft.AspNetCore.Mvc;
using LEIUTAD.Data;
using LEIUTAD.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LEIUTAD.Controllers
{
    public class EmprestimosController : Controller
    {
        private readonly LEIUTADContext _context;

        public EmprestimosController(LEIUTADContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FinalizarEmprestimo()
        {
            var leitorId = HttpContext.Session.GetInt32("UserId");
            var carrinho = HttpContext.Session.GetObjectFromJson<List<string>>("Carrinho") ?? new List<string>();

            if (leitorId == null)
            {
                TempData["MensagemErro"] = "O utilizador não está autenticado.";
                return RedirectToAction("Login", "Account");
            }      

            if (!carrinho.Any())
            {
                TempData["MensagemErro"] = "O carrinho está vazio.";
                return RedirectToAction("VerCarrinho", "Leitor");
            }

            // Verificar se o leitor já tem um empréstimo "Por devolver"
            var emprestimoPendente = _context.Emprestimo
                .Any(e => e.ID_Leitor == leitorId.Value && e.Estado == "Por devolver");

            if (emprestimoPendente)
            {
                TempData["MensagemErro"] = "Não pode finalizar um novo empréstimo enquanto tiver um empréstimo pendente para devolução.";
                return RedirectToAction("VerCarrinho", "Leitor");
            }

            try
            {
                var emprestimo = new Emprestimo
                {
                    ID_Leitor = leitorId.Value,
                    Data_Req = DateTime.Now,
                    Data_Dev = DateTime.Now.AddDays(14)
                };

                _context.Emprestimo.Add(emprestimo);
                _context.SaveChanges();

                foreach (var isbn in carrinho)
                {
                    _context.Emprestimo_Livro.Add(new Emprestimo_Livro
                    {
                        ID_Emp = emprestimo.ID_Emp,
                        ISBN = isbn
                    });
                }

                _context.SaveChanges();

                HttpContext.Session.Remove("Carrinho");
                HttpContext.Session.SetInt32("CarrinhoCount", 0);

                TempData["MensagemSucesso"] = "Empréstimo concluído com sucesso.";
                return RedirectToAction("VerCarrinho", "Leitor");
            }
            catch (Exception ex)
            {
                var detailedError = ex.InnerException != null ? ex.InnerException.Message : "Sem detalhes adicionais.";
                System.IO.File.AppendAllText("log_emprestimo.txt", $"Erro ao salvar na base de dados: {ex.Message}\nDetalhes: {detailedError}\nStackTrace: {ex.StackTrace}\n");
                TempData["MensagemErro"] = $"Erro ao concluir empréstimo: {ex.Message}";
                return RedirectToAction("VerCarrinho", "Leitor");
            }
        }
    }
}
