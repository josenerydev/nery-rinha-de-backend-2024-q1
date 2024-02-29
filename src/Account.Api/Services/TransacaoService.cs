using Account.Api.Data;
using Account.Api.Dtos;
using Account.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Account.Api.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly AccountApiContext _context;

        public TransacaoService(AccountApiContext context)
        {
            _context = context;
        }

        public async Task<(bool, Cliente?, string)> RealizarTransacao(int clienteId, int valor, char tipo, string descricao)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                return (false, null, "Cliente não encontrado.");
            }

            if (tipo == 'd')
            {
                if (cliente.Saldo - valor < -cliente.Limite)
                {
                    return (false, cliente, "Saldo insuficiente.");
                }
                cliente.Saldo -= valor;
            }
            else if (tipo == 'c')
            {
                cliente.Saldo += valor;
            }

            _context.Transacoes.Add(new Transacao
            {
                Valor = tipo == 'c' ? valor : -valor,
                Tipo = tipo,
                Descricao = descricao,
                RealizadaEm = DateTime.UtcNow,
                ClienteId = clienteId
            });

            await _context.SaveChangesAsync();
            return (true, cliente, string.Empty);
        }

        public async Task<ExtratoRespostaDto> ObterExtrato(int clienteId)
        {
            var clienteExists = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
            if (!clienteExists) return null!;

            var ultimasTransacoes = await _context.Transacoes
                .Where(t => t.ClienteId == clienteId)
                .OrderByDescending(t => t.RealizadaEm)
                .Take(10)
                .Select(t => new TransacaoDto
                {
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    Descricao = t.Descricao,
                    RealizadaEm = t.RealizadaEm
                }).ToListAsync();

            var cliente = await _context.Clientes
                .Where(c => c.Id == clienteId)
                .Select(c => new { c.Saldo, c.Limite })
                .FirstOrDefaultAsync();

            if (cliente == null) return null!;

            var extratoRespostaDto = new ExtratoRespostaDto
            {
                Saldo = new SaldoDto
                {
                    Total = cliente.Saldo,
                    DataExtrato = DateTime.UtcNow,
                    Limite = cliente.Limite
                },
                UltimasTransacoes = ultimasTransacoes
            };

            return extratoRespostaDto;
        }
    }
}