using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace LEIUTAD.Models
{
    public class Livros
    {
        [Key]
        public int ISBM { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um título.")]
        [StringLength(150, ErrorMessage = "O título deve conter no máximo 150 caracteres.")]
        public string Titulo { get; set; }
        public int ID_Autor { get; set; }
        public Autores Autor { get; set; }
        public int ID_Genero { get; set; }
        public Generos Genero { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar o preço do livro.")]
        public float Preco { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar o número de exemplares.")]
        public string N_Exemplares { get; set; }

        [StringLength(1000, ErrorMessage = "A sinopse pode ter no máximo 1000 caracteres.")]
        public string Sinopse { get; set; }
        public string Imagem { get; set; }
    }
}
