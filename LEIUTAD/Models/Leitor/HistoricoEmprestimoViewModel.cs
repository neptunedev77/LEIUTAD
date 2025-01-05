namespace LEIUTAD.ViewModels
{
    public class HistoricoEmprestimoViewModel
    {
        public int IDEmprestimo { get; set; }
        public DateTime DataRequisicao { get; set; }
        public DateTime DataDevolucao { get; set; }
        public string Estado { get; set; }
        public List<string> Livros { get; set; } = new List<string>();
    }
}

