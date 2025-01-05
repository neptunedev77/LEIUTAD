using Microsoft.AspNetCore.Mvc;
using LEIUTAD.Data;
using LEIUTAD.Filters;
using LEIUTAD.Models.Administrador;

namespace LEIUTAD.Controllers
{
    [AdministradorAuthorize] // Filtro aplicado ao controlador inteiro
    public class AdministradorController : Controller
    {
        private readonly LEIUTADContext _context;

        public AdministradorController(LEIUTADContext context)
        {
            _context = context;
        }

        // Painel inicial do administrador
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = "Bem-vindo ao Painel do Administrador!";
            return View();
        }

        // Ação para carregar a página GerirUtilizadores
        [HttpGet]
        public IActionResult GerirUtilizadores()
        {
            var utilizadores = _context.Leitor
                .Select(l => new UtilizadorViewModel
                {
                    ID = l.ID_user,
                    Nome = l.Nome,
                    Cargo = "Leitor"
                })
                .Union(
                    _context.Bibliotecarios.Select(b => new UtilizadorViewModel
                    {
                        ID = b.ID_Bib,
                        Nome = b.Nome,
                        Cargo = "Bibliotecário"
                    })
                )
                .ToList();

            return View(utilizadores);
        }

        [HttpGet]
        public IActionResult DetalhesUtilizador(int id, string tipo)
        {
            if (tipo == "Leitor")
            {
                var leitor = _context.Leitor.FirstOrDefault(l => l.ID_user == id);
                if (leitor == null)
                {
                    return Json(new { sucesso = false, mensagem = "Leitor não encontrado." });
                }

                return PartialView("_DetalhesUtilizadorModal", new
                {
                    Tipo = "Leitor",
                    Detalhes = leitor
                });
            }
            else if (tipo == "Bibliotecário")
            {
                var bibliotecario = _context.Bibliotecarios.FirstOrDefault(b => b.ID_Bib == id);
                if (bibliotecario == null)
                {
                    return Json(new { sucesso = false, mensagem = "Bibliotecário não encontrado." });
                }

                return PartialView("_DetalhesUtilizadorModal", new
                {
                    Tipo = "Bibliotecário",
                    Detalhes = bibliotecario
                });
            }
            else
            {
                return Json(new { sucesso = false, mensagem = "Tipo de utilizador inválido." });
            }
        }



    }
}
