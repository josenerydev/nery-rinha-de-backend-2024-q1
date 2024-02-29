namespace Account.Api.Dtos
{
    public class TransacaoRequisicaoDto
    {
        public int Valor { get; set; }
        public char Tipo { get; set; }
        public string? Descricao { get; set; }
    }
}