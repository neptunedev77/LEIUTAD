using LEIUTAD.Data;
using LEIUTAD.Models;
using LEIUTAD.Filters; // Certifique-se de que este namespace está correto para o filtro
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    }
}
