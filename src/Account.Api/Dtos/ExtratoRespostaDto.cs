using System.Text.Json.Serialization;

namespace Account.Api.Dtos
{
    public class ExtratoRespostaDto
    {
        [JsonPropertyName("saldo")]
        public SaldoDto Saldo { get; set; }

        [JsonPropertyName("ultimas_transacoes")]
        public List<TransacaoDto> UltimasTransacoes { get; set; }

        public ExtratoRespostaDto()
        {
            UltimasTransacoes = new List<TransacaoDto>();
        }
    }
}