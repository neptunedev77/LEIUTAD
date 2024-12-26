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

        //Para passar para a página dos detalhes do livro
        [HttpGet]
        public IActionResult Detalhes(int id, string nome)
        {
            // Busca o livro pelo ID
            var livro = _context.Livro
                .Include(l => l.Autor)
                .Include(l => l.Genero)
                .FirstOrDefault(l => l.ISBM == id);

            // Verifica se o livro existe
            if (livro == null)
            {
                return NotFound(); // Retorna erro 404 se o livro não existir
            }

            // Passa o livro para a view
            return View(livro);
        }
    }
}
