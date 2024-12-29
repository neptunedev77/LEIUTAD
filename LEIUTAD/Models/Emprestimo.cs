using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.Models
{
    public class Emprestimo
    {
        [Key]
        public int ID_Emp { get; set; }
        public int ID_Leitor { get; set; }
        public DateTime Data_Req { get; set; }
        public DateTime Data_Dev { get; set; }
        public string Estado { get; set; } = "Por Devolver";

        public Leitor Leitor { get; set; }
        public ICollection<Emprestimo_Livro> Emprestimo_Livros { get; set; }
    }
}