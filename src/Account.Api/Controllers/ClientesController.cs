using Account.Api.Dtos;
using Account.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly TransacaoService _transacaoService;

        public ClientesController(TransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        [HttpPost("{id}/transacoes")]
        public async Task<IActionResult> PostTransacao(int id, TransacaoRequisicaoDto transacaoRequisicaoDTO)
        {
            (bool sucesso, var cliente) = transacaoRequisicaoDTO.Tipo == 'c' ?
                await _transacaoService.RealizarDeposito(id, transacaoRequisicaoDTO.Valor, transacaoRequisicaoDTO.Descricao) :
                await _transacaoService.RealizarSaque(id, transacaoRequisicaoDTO.Valor, transacaoRequisicaoDTO.Descricao);

            if (!sucesso)
            {
                if (cliente == null) return NotFound("Cliente não encontrado.");
                return UnprocessableEntity("Não foi possível realizar a transação.");
            }

            return Ok(new TransacaoRespostaDto { Limite = cliente.Limite, Saldo = cliente.Saldo });
        }

        [HttpGet("{id}/extrato")]
        public async Task<IActionResult> GetExtrato(int id)
        {
            var extrato = await _transacaoService.ObterExtrato(id);

            if (extrato == null) return NotFound("Cliente não encontrado.");

            return Ok(extrato);
        }
    }
}