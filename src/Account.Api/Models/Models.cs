using System.Text.Json.Serialization;

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
        public char Tipo { get; set; }
        public string Descricao { get; set; }
    }

    public class TransacaoRespostaDTO
    {
        public int Limite { get; set; }
        public int Saldo { get; set; }
    }

    public class ExtratoRespostaDTO
    {
        [JsonPropertyName("saldo")]
        public SaldoDTO Saldo { get; set; }

        [JsonPropertyName("ultimas_transacoes")]
        public List<TransacaoDTO> UltimasTransacoes { get; set; }

        public ExtratoRespostaDTO()
        {
            UltimasTransacoes = new List<TransacaoDTO>();
        }
    }

    public class SaldoDTO
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("data_extrato")]
        public DateTime DataExtrato { get; set; }

        [JsonPropertyName("limite")]
        public int Limite { get; set; }
    }

    public class TransacaoDTO
    {
        [JsonPropertyName("valor")]
        public int Valor { get; set; }

        [JsonPropertyName("tipo")]
        public char Tipo { get; set; } // 'c' para crédito, 'd' para débito

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("realizada_em")]
        public DateTime RealizadaEm { get; set; }
    }
}