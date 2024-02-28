using System.Text.Json.Serialization;

namespace Account.Api.Dtos
{
    public class SaldoDTO
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("data_extrato")]
        public DateTime DataExtrato { get; set; }

        [JsonPropertyName("limite")]
        public int Limite { get; set; }
    }
}