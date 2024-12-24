using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
