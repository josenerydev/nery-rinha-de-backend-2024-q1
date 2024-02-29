using System.Collections.Concurrent;

namespace Yarp.LoadBalancer
{
    public class ConcurrencyControlMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();

        public ConcurrencyControlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Verifica se a requisição é para a rota de transações
            if (path.EndsWith("/transacoes"))
            {
                var clientId = ExtractClientIdFromPath(path);
                if (clientId.HasValue)
                {
                    var clientLock = _locks.GetOrAdd(clientId.Value, _ => new SemaphoreSlim(1, 1));
                    try
                    {
                        await clientLock.WaitAsync();
                        await _next(context);
                    }
                    finally
                    {
                        clientLock.Release();
                    }
                }
                else
                {
                    // Se o clientId não for válido, prossiga sem bloqueio
                    await _next(context);
                }
            }
            else
            {
                // Rotas que não são de transações passam sem bloqueio
                await _next(context);
            }
        }

        private int? ExtractClientIdFromPath(string path)
        {
            var segments = path.Split('/');
            // Espera-se que o caminho seja algo como "/clientes/{clientId}/transacoes"
            if (segments.Length >= 3 && segments[^2].Equals("clientes", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(segments[^3], out var clientId))
                {
                    return clientId;
                }
            }
            return null;
        }
    }
}