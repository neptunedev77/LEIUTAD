using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "É obrigatório colocar um nome.")]
        [StringLength(150, ErrorMessage = "O nome deve conter no máximo 150 caracteres.")]
        [MinLength(2, ErrorMessage = "O nome deve conter no mínimo 2 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar uma password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "É obrigatório confirmar a password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar um Email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar um número de telefone.")]
        public string Tele_n { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar uma data de nascimento.")]
        [DataType(DataType.Date)]
        public DateTime Data_n { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar uma morada.")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar um número.")]
        public string Endereco_n { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar uma localidade.")]
        public string Localidade { get; set; }

        [Required(ErrorMessage = "É obrigatório colocar um país.")]
        public string Pais { get; set; }
    }
}