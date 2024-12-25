using LEIUTAD.Data;
using LEIUTAD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LEIUTAD.Controllers
{
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

            var livros = _context.Livro.ToList(); // Obtem os livros
            return View(livros);
        }
    }
}
