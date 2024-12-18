using LEIUTAD.Data;
using LEIUTAD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LEIUTAD.Controllers
{
    public class LivrosController : Controller
    {
        private readonly LEIUTADContext _context;
        public LivrosController(LEIUTADContext context) {
            _context = context;
        }
        public IActionResult Index()
        {
            // Carregar os livros organizados por género
            var livrosPorGenero = _context.Genero
                .Select(g => new LivrosPorGeneroViewModel
                {
                    Genero = g.Genero,
                    Livros = _context.Livro
                        .Where(l => l.ID_Genero == g.ID_Genero)
                        .Include(l => l.Autor) // Carregar a propriedade Autor
                        .ToList()
                })
                .ToList();

            return View(livrosPorGenero);
        }
    }
}
