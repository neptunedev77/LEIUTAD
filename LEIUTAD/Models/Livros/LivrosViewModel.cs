using LEIUTAD.Models;
using System.Collections.Generic;

namespace LEIUTAD.ViewModels
{
    public class LivrosPorGeneroViewModel
    {
        public string Genero { get; set; }
        public List<Livros> Livros { get; set; }
    }
}