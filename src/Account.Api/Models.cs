namespace Account.Api
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

    public class Transacao
    {
        public int Id { get; set; }
        public int Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public string Descricao { get; set; }
        public DateTime RealizadaEm { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }

    public enum TipoTransacao
    {
        Credito = 'c',
        Debito = 'd'
    }

    public class Extrato
    {
        public int Id { get; set; }
        public int SaldoTotal { get; set; }
        public DateTime DataExtrato { get; set; }
        public int Limite { get; set; }
        public List<Transacao> UltimasTransacoes { get; set; }

        public Extrato()
        {
            UltimasTransacoes = new List<Transacao>();
        }
    }

    public class TransacaoRequisicaoDTO
    {
        public int Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public string Descricao { get; set; }
    }

    public class ExtratoRespostaDTO
    {
        public SaldoDTO Saldo { get; set; }
        public List<TransacaoDTO> UltimasTransacoes { get; set; }

        public ExtratoRespostaDTO()
        {
            UltimasTransacoes = new List<TransacaoDTO>();
        }
    }

    public class SaldoDTO
    {
        public int Total { get; set; }
        public DateTime DataExtrato { get; set; }
        public int Limite { get; set; }
    }

    public class TransacaoDTO
    {
        public int Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public string Descricao { get; set; }
        public DateTime RealizadaEm { get; set; }
    }
}