using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using LEIUTAD.Data;
using LEIUTAD.Models;
using Microsoft.EntityFrameworkCore;

namespace LEIUTAD.Controllers
{
    public class PesquisaController : Controller
    {
        private readonly LEIUTADContext _context;

        public PesquisaController(LEIUTADContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewData["Mensagem"] = "Por favor, insira um termo para pesquisa.";
                return View(new List<Livros>());
            }

            var resultados = _context.Livro
                .Include(l => l.Autor) // Inclua o relacionamento Autor
                .Include(l => l.Genero) // Inclua também Genero, se necessário
                .Where(l => l.Titulo.Contains(query))
                .ToList();

            if (!resultados.Any())
            {
                ViewData["Mensagem"] = $"Nenhum resultado encontrado para '{query}'.";
            }

            return View(resultados);
        }
    }
}
