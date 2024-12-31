using System.ComponentModel.DataAnnotations;

namespace LEIUTAD.Models
{
    public class Leitor
    {

        [Key]
        public int ID_user { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um nome.")]
        [StringLength(150, ErrorMessage = "O nome deve conter no máximo 150 caracteres.")]
        [MinLength(2, ErrorMessage = "O nome deve conter no mínimo 2 caracteres.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar uma password.")]
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um número de telefone.")]
        public string Tele_n { get; set; }
        public DateTime Data_n { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar uma morada.")]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um número.")]
        public string Endereco_n { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar uma localidade.")]
        public string Localidade { get; set; }
        [Required(ErrorMessage = "É obrigatório colocar um país.")]
        public string Pais { get; set; }
        public bool Estado { get; set; } = false; // False por padrão, conta não verificada
        public bool IsBloqueado { get; set; } = false; // False significa desbloqueado por padrão
        public string? TokenVerificacao { get; set; }

        public ICollection<Emprestimo> Emprestimos { get; set; }
    }
}
