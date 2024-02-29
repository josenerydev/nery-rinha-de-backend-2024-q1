using System.Text.Json.Serialization;

namespace Account.Api.Dtos
{
    public class TransacaoDto
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