using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LEIUTAD.Models;

namespace LEIUTAD.Data
{
    public class LEIUTADContext : DbContext
    {
        public LEIUTADContext (DbContextOptions<LEIUTADContext> options)
            : base(options)
        {
        }

        public DbSet<LEIUTAD.Models.Leitor> Leitor { get; set; } = default!;
        public DbSet<LEIUTAD.Models.Autores> Autor { get; set; } = default!;
        public DbSet<LEIUTAD.Models.Livros> Livro { get; set; } = default!;
        public DbSet<LEIUTAD.Models.Generos> Genero { get; set; } = default!;
        public DbSet<LEIUTAD.Models.Administradores> Administrador { get; set; } = default!;
        public DbSet<LEIUTAD.Models.Bibliotecarios> Bibliotecarios { get; set; } = default!;

    }
}
