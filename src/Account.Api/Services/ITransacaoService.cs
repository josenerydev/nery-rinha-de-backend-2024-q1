using Account.Api.Dtos;
using Account.Api.Models;

namespace Account.Api.Services
{
    public interface ITransacaoService
    {
        Task<(bool, Cliente?, string)> RealizarTransacao(int clienteId, int valor, char tipo, string descricao);

        Task<ExtratoRespostaDto> ObterExtrato(int clienteId);
    }
}