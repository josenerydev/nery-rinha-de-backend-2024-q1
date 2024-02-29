using Account.Api.Dtos;

namespace Account.Api.Services
{
    public class TransacaoValidacaoService : ITransacaoValidacaoService
    {
        public ValidationResult ValidarValor(int valor)
        {
            if (valor <= 0)
            {
                return new ValidationResult(false, "Valor deve ser positivo e inteiro.");
            }

            return new ValidationResult(true);
        }

        public ValidationResult ValidarTipo(char tipo)
        {
            if (tipo != 'c' && tipo != 'd')
            {
                return new ValidationResult(false, "Tipo deve ser 'c' para crédito ou 'd' para débito.");
            }

            return new ValidationResult(true);
        }

        public ValidationResult ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao) || descricao.Length > 10)
            {
                return new ValidationResult(false, "Descrição inválida; deve ter entre 1 e 10 caracteres.");
            }

            return new ValidationResult(true);
        }
    }
}