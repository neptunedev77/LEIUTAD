namespace LEIUTAD.Models
{
    public class Emprestimo_Livro
    {
        public int ID_Emp { get; set; }
        public string ISBN { get; set; }

        public Emprestimo Emprestimo { get; set; }
        public Livros Livro { get; set; }
    }

}
