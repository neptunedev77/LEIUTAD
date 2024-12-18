using LEIUTAD.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LEIUTAD.Data
{
    public class DBInitialize
    {
        public static void Initialize(LEIUTADContext contextoDB)
        {
            contextoDB.Database.EnsureCreated();

            // Verificar se já existem autores na base de dados
            if (contextoDB.Autor.Any())
            {
                Console.WriteLine("Base de dados já foi preenchida");
                return;
            }

            // Criar exemplos de autores
            var autores = new Autores[]
            {
                new Autores
                {
                    Autor_Nome = "José Saramago",
                    Data_n = new DateTime(1922, 11, 16),
                    Pais = "Portugal",
                    Biografia = "Prémio Nobel da Literatura em 1998."
                },
                new Autores
                {
                    Autor_Nome = "Eça de Queirós",
                    Data_n = new DateTime(1845, 11, 25),
                    Pais = "Portugal",
                    Biografia = "Considerado um dos maiores romancistas em língua portuguesa."
                }
            };

            contextoDB.Autor.AddRange(autores);
            contextoDB.SaveChanges();

            // Criar exemplos de géneros
            var generos = new Generos[]
            {
                new Generos { Genero = "Ficção" },
                new Generos { Genero = "Romance" },
                new Generos { Genero = "História" }
            };

            contextoDB.Genero.AddRange(generos);
            contextoDB.SaveChanges();

            // Criar exemplos de livros
            var livros = new Livros[]
            {
                new Livros
                {
                    Titulo = "Ensaio sobre a Cegueira",
                    ID_Autor = autores[0].ID_Autor,
                    Autor = autores[0],
                    ID_Genero = generos[0].ID_Genero,
                    Genero = generos[0],
                    Preco = 15.99f,
                    N_Exemplares = "10"
                },
                new Livros
                {
                    Titulo = "Os Maias",
                    ID_Autor = autores[1].ID_Autor,
                    Autor = autores[1],
                    ID_Genero = generos[1].ID_Genero,
                    Genero = generos[1],
                    Preco = 18.50f,
                    N_Exemplares = "5"
                }
            };

            contextoDB.Livro.AddRange(livros);
            contextoDB.SaveChanges();
        }
    }
}