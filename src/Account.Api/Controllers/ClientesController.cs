using Account.Api.Dtos;
using Account.Api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using RedLockNet;

namespace Account.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ITransacaoValidacaoService _transacaoValidacaoService;
        private readonly DistributedLockFactoryOptions _distributedLockFactoryOptions;
        private readonly IDistributedLockFactory _lockFactory;

        public ClientesController(ITransacaoService transacaoService, ITransacaoValidacaoService transacaoValidacaoService, IDistributedLockFactory lockFactory, IOptions<DistributedLockFactoryOptions> distributedLockFactoryOptions)
        {
            _transacaoService = transacaoService;
            _transacaoValidacaoService = transacaoValidacaoService;
            _distributedLockFactoryOptions = distributedLockFactoryOptions.Value;
            _lockFactory = lockFactory;
        }

        [HttpPost("{id}/transacoes")]
        public async Task<IActionResult> PostTransacao(int id, TransacaoRequisicaoDto transacaoRequisicaoDTO)
        {
            string chaveBloqueio = $"cliente:{id}";

            await using var _lock = await _lockFactory.CreateLockAsync(
                chaveBloqueio,
                TimeSpan.FromSeconds(_distributedLockFactoryOptions.ExpiryTime),
                TimeSpan.FromSeconds(_distributedLockFactoryOptions.WaitTime),
                TimeSpan.FromSeconds(_distributedLockFactoryOptions.RetryTime)
            );

            if (_lock.IsAcquired)
            {
                var tipoValido = _transacaoValidacaoService.ValidarTipo(transacaoRequisicaoDTO.Tipo);
                if (!tipoValido.IsValid)
                    return UnprocessableEntity(tipoValido.ErrorMessage);

                var valorValido = _transacaoValidacaoService.ValidarValor(transacaoRequisicaoDTO.Valor);
                if (!valorValido.IsValid)
                    return UnprocessableEntity(valorValido.ErrorMessage);

                var descricaoValida = _transacaoValidacaoService.ValidarDescricao(transacaoRequisicaoDTO.Descricao);
                if (!descricaoValida.IsValid)
                    return UnprocessableEntity(descricaoValida.ErrorMessage);

                (bool sucesso, var cliente, string erro) = await _transacaoService.RealizarTransacao(id, transacaoRequisicaoDTO.Valor, transacaoRequisicaoDTO.Tipo, transacaoRequisicaoDTO.Descricao);

                if (!sucesso)
                {
                    if (cliente == null)
                    {
                        return NotFound("Cliente não encontrado.");
                    }
                    else if (erro == "Saldo insuficiente.")
                    {
                        return UnprocessableEntity("Transação excede o limite do cliente.");
                    }
                }

                return Ok(new TransacaoRespostaDto { Limite = cliente.Limite, Saldo = cliente.Saldo });
            }

            return StatusCode(StatusCodes.Status423Locked, "O recurso está temporariamente bloqueado por outra operação.");
        }

        [HttpGet("{id}/extrato")]
        public async Task<IActionResult> GetExtrato(int id)
        {
            string chaveBloqueio = $"cliente:{id}";

            await using var _lock = await _lockFactory.CreateLockAsync(
                chaveBloqueio,
                TimeSpan.FromSeconds(_distributedLockFactoryOptions.ExpiryTime),
                TimeSpan.FromSeconds(_distributedLockFactoryOptions.WaitTime),
                TimeSpan.FromSeconds(_distributedLockFactoryOptions.RetryTime)
            );

            if (_lock.IsAcquired)
            {
                var extrato = await _transacaoService.ObterExtrato(id);

                if (extrato == null) return NotFound("Cliente não encontrado.");

                return Ok(extrato);
            }
            return StatusCode(StatusCodes.Status423Locked, "O recurso está temporariamente bloqueado por outra operação.");
        }
    }
}