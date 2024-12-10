using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.Models
{
    public class Autores
    {
        [Key]
        public int ID_Autor { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um nome.")]
        [StringLength(150, ErrorMessage = "O nome deve conter no máximo 150 caracteres.")]
        [MinLength(2, ErrorMessage = "O nome deve conter no mínimo 2 caracteres.")]
        public string Autor_Nome { get; set; }
        public DateTime Data_n { get; set; }
        public string Pais { get; set; }
        public string Biografia { get; set; }
    }
}
