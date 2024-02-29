using Account.Api.Dtos;

namespace Account.Api.Services
{
    public interface ITransacaoValidacaoService
    {
        ValidationResult ValidarValor(int valor);

        ValidationResult ValidarTipo(char tipo);

        ValidationResult ValidarDescricao(string descricao);
    }
}