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
        public LEIUTADContext(DbContextOptions<LEIUTADContext> options)
            : base(options)
        {
        }

        public DbSet<Leitor> Leitor { get; set; } = default!;
        public DbSet<Autores> Autor { get; set; } = default!;
        public DbSet<Livros> Livro { get; set; } = default!;
        public DbSet<Generos> Genero { get; set; } = default!;
        public DbSet<Administradores> Administrador { get; set; } = default!;
        public DbSet<Bibliotecarios> Bibliotecarios { get; set; } = default!;
        public DbSet<Emprestimo> Emprestimo { get; set; }
        public DbSet<Emprestimo_Livro> Emprestimo_Livro { get; set; }

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

            // Configuração adicional
            modelBuilder.Entity<Livros>()
                .HasKey(l => l.ISBN);

            // Configurar Emprestimos
            modelBuilder.Entity<Emprestimo>()
                .HasKey(e => e.ID_Emp); // Define a chave primária para Emprestimos

            modelBuilder.Entity<Emprestimo>()
                .HasOne(e => e.Leitor) // Um Emprestimo pertence a um Leitor
                .WithMany(l => l.Emprestimos)
                .HasForeignKey(e => e.ID_Leitor)
                .OnDelete(DeleteBehavior.Cascade); // Apaga os empréstimos se o leitor for apagado

            // Configuração para a relação Emprestimo <-> Emprestimo_Livro
            modelBuilder.Entity<Emprestimo_Livro>()
                .HasKey(el => new { el.ID_Emp, el.ISBN }); // Configura uma chave composta para a tabela associativa

            modelBuilder.Entity<Emprestimo_Livro>()
                .HasOne(el => el.Emprestimo) // Um Emprestimo_Livro está associado a um Emprestimo
                .WithMany(e => e.Emprestimo_Livros)
                .HasForeignKey(el => el.ID_Emp)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Emprestimo_Livro>()
                .HasOne(el => el.Livro) // Um Emprestimo_Livro está associado a um Livro
                .WithMany()
                .HasForeignKey(el => el.ISBN)
                .OnDelete(DeleteBehavior.Restrict); // Não apaga livros ao excluir Emprestimo_Livro
        }
    }
}
