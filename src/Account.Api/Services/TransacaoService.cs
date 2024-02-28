using Account.Api.Data;
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

        public async Task<(bool, Cliente)> RealizarDeposito(int clienteId, int valor, string descricao)
        {
            var cliente = await _context.Clientes.Include(c => c.Transacoes).FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente == null) return (false, null); // Cliente não encontrado

            cliente.Saldo += valor;
            cliente.Transacoes.Add(new Transacao
            {
                Valor = valor,
                Tipo = TipoTransacao.Credito,
                Descricao = descricao,
                RealizadaEm = DateTime.UtcNow,
                ClienteId = clienteId
            });

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return (true, cliente);
        }

        public async Task<(bool, Cliente)> RealizarSaque(int clienteId, int valor, string descricao)
        {
            var cliente = await _context.Clientes.Include(c => c.Transacoes).FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente == null) return (false, null); // Cliente não encontrado

            if (cliente.Saldo - valor < -cliente.Limite) return (false, cliente); // Saque deixaria o saldo abaixo do limite

            cliente.Saldo -= valor;
            cliente.Transacoes.Add(new Transacao
            {
                Valor = -valor,
                Tipo = TipoTransacao.Debito,
                Descricao = descricao,
                RealizadaEm = DateTime.UtcNow,
                ClienteId = clienteId
            });

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return (true, cliente);
        }

        public async Task<ExtratoRespostaDTO> ObterExtrato(int clienteId)
        {
            var cliente = await _context.Clientes
                                        .AsNoTracking()
                                        .Include(c => c.Transacoes)
                                        .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null) return null; // Cliente não encontrado

            var ultimasTransacoes = cliente.Transacoes
                                           .OrderByDescending(t => t.RealizadaEm)
                                           .Select(t => new TransacaoDTO
                                           {
                                               Valor = t.Valor,
                                               Tipo = (char)t.Tipo,
                                               Descricao = t.Descricao,
                                               RealizadaEm = t.RealizadaEm
                                           }).ToList();

            var extratoRespostaDTO = new ExtratoRespostaDTO
            {
                Saldo = new SaldoDTO
                {
                    Total = cliente.Saldo,
                    DataExtrato = DateTime.UtcNow,
                    Limite = cliente.Limite
                },
                UltimasTransacoes = ultimasTransacoes
            };

            return extratoRespostaDTO;
        }
    }
}