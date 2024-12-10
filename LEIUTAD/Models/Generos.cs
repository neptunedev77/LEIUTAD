using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.Models
{
    public class Generos
    {
        [Key]
        public int ID_Genero { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um género.")]
        [StringLength(150, ErrorMessage = "O género deve conter no máximo 150 caracteres.")]
        public string Genero { get; set; }
    }
}
