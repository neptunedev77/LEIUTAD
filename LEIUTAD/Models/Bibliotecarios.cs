using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.Models
{
    public class Bibliotecarios
    {
        [Key]
        public int ID_Bib { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um nome.")]
        [StringLength(150, ErrorMessage = "O nome deve conter no máximo 150 caracteres.")]
        [MinLength(2, ErrorMessage = "O nome deve conter no mínimo 2 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar um Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar uma password.")]
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar um contacto.")]
        public string Tele_n { get; set; }
    }
}
