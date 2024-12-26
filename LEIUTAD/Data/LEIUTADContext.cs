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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relação Livros -> Autores (Muitos para Um)
            modelBuilder.Entity<Livros>()
                .HasOne(l => l.Autor)
                .WithMany()
                .HasForeignKey(l => l.ID_Autor)
                .OnDelete(DeleteBehavior.Cascade); // Apagar livros se o autor for apagado

            // Relação Livros -> Generos (Muitos para Um)
            modelBuilder.Entity<Livros>()
                .HasOne(l => l.Genero)
                .WithMany()
                .HasForeignKey(l => l.ID_Genero)
                .OnDelete(DeleteBehavior.Restrict); // Não apagar géneros quando livros são apagados

            // Configuração adicional (se necessário):
            modelBuilder.Entity<Livros>()
                .HasKey(l => l.ISBN);

            // Adicione outras configurações de relações aqui, se necessário.
        }

    }
}
