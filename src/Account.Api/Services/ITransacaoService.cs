using Account.Api.Dtos;
using Account.Api.Models;

namespace Account.Api.Services
{
    public interface ITransacaoService
    {
        Task<ExtratoRespostaDto> ObterExtrato(int clienteId);

        Task<(bool, Cliente, string)> RealizarDeposito(int clienteId, int valor, string descricao);

        Task<(bool, Cliente, string)> RealizarSaque(int clienteId, int valor, string descricao);
    }
}