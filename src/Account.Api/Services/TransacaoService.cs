using Account.Api.Data;
using Account.Api.Dtos;
using Account.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Account.Api.Services
{
    public class TransacaoService
    {
        private readonly AccountApiContext _context;

        public TransacaoService(AccountApiContext context)
        {
            _context = context;
        }

        public async Task<(bool, Cliente, string)> RealizarDeposito(int clienteId, int valor, string descricao)
        {
            if (valor <= 0) return (false, null, "Valor deve ser positivo e inteiro.");
            if (string.IsNullOrWhiteSpace(descricao) || descricao.Length > 10) return (false, null, "Descrição inválida.");

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return (false, null, "Cliente não encontrado.");

            cliente.Saldo += valor;
            cliente.Transacoes.Add(new Transacao
            {
                Valor = valor,
                Tipo = 'c',
                Descricao = descricao,
                RealizadaEm = DateTime.UtcNow,
                ClienteId = clienteId
            });

            await _context.SaveChangesAsync();
            return (true, cliente, string.Empty);
        }

        public async Task<(bool, Cliente, string)> RealizarSaque(int clienteId, int valor, string descricao)
        {
            if (valor <= 0) return (false, null, "Valor deve ser positivo e inteiro.");
            if (string.IsNullOrWhiteSpace(descricao) || descricao.Length > 10) return (false, null, "Descrição inválida.");

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return (false, null, "Cliente não encontrado.");

            if (cliente.Saldo - valor < -cliente.Limite) return (false, cliente, "Transação excede o limite do cliente.");

            cliente.Saldo -= valor;
            cliente.Transacoes.Add(new Transacao
            {
                Valor = -valor,
                Tipo = 'd',
                Descricao = descricao,
                RealizadaEm = DateTime.UtcNow,
                ClienteId = clienteId
            });

            await _context.SaveChangesAsync();
            return (true, cliente, string.Empty);
        }

        public async Task<ExtratoRespostaDto> ObterExtrato(int clienteId)
        {
            // Verifica se o cliente existe
            var clienteExists = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
            if (!clienteExists) return null!;

            // Consulta as 10 últimas transações do cliente diretamente do banco de dados
            var ultimasTransacoes = await _context.Transacoes
                                                  .AsNoTracking()
                                                  .Where(t => t.ClienteId == clienteId)
                                                  .OrderByDescending(t => t.RealizadaEm)
                                                  .Take(10)
                                                  .Select(t => new TransacaoDto
                                                  {
                                                      Valor = t.Valor,
                                                      Tipo = (char)t.Tipo,
                                                      Descricao = t.Descricao,
                                                      RealizadaEm = t.RealizadaEm
                                                  }).ToListAsync();

            // Consulta o saldo e limite do cliente
            var cliente = await _context.Clientes
                                        .AsNoTracking()
                                        .Select(c => new { c.Id, c.Saldo, c.Limite })
                                        .FirstOrDefaultAsync(c => c.Id == clienteId);

            var extratoRespostaDTO = new ExtratoRespostaDto
            {
                Saldo = new SaldoDto
                {
                    Total = cliente!.Saldo,
                    DataExtrato = DateTime.UtcNow,
                    Limite = cliente.Limite
                },
                UltimasTransacoes = ultimasTransacoes
            };

            return extratoRespostaDTO;
        }
    }
}