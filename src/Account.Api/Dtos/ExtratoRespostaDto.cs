using System.Text.Json.Serialization;

namespace Account.Api.Dtos
{
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
}