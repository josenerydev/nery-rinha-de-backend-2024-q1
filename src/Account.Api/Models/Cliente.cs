namespace Account.Api.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public int Limite { get; set; }
        public int Saldo { get; set; }
        public List<Transacao> Transacoes { get; set; }

        public Cliente()
        {
            Transacoes = new List<Transacao>();
        }
    }
}