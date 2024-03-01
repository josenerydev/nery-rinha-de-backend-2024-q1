using Account.Api.Data;
using Account.Api.Dtos;
using Account.Api.Models;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using System.Net;
using System.Text;

namespace Account.Tests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly AccountApiContext _context;

        public IntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<AccountApiContext>();

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            ClearDatabase();

            var clientes = new List<Cliente>
            {
                new Cliente { Id = 1, Limite = 100000, Saldo = 0 },
                new Cliente { Id = 2, Limite = 80000, Saldo = 0 },
                new Cliente { Id = 3, Limite = 1000000, Saldo = 0 },
                new Cliente { Id = 4, Limite = 10000000, Saldo = 0 },
                new Cliente { Id = 5, Limite = 500000, Saldo = 0 }
            };

            _context.Clientes.AddRange(clientes);
            _context.SaveChanges();
        }

        private void ClearDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PostTransacao_TransacaoValida_DeveRetornarSucesso()
        {
            var transacaoDto = new TransacaoRequisicaoDto { Valor = 1000, Tipo = 'c', Descricao = "descricao" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = JsonConvert.DeserializeObject<TransacaoRespostaDto>(await response.Content.ReadAsStringAsync());

            responseData.Limite.Should().Be(100000);
            responseData.Saldo.Should().Be(1000);
        }

        [Fact]
        public async Task PostTransacao_TransacaoDebitoExcedeLimite_DeveRetornarErro()
        {
            var transacaoDto = new TransacaoRequisicaoDto { Valor = 110000, Tipo = 'd', Descricao = "descricao" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task PostTransacao_ClienteNaoExistente_DeveRetornarNotFound()
        {
            var transacaoDto = new TransacaoRequisicaoDto { Valor = 1000, Tipo = 'c', Descricao = "descricao" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/6/transacoes", content); // Usando o ID 6, que não deve existir

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetExtrato_DeveRetornarExtratoCorreto()
        {
            // Arrange: Preparação do ambiente de teste
            var transacaoDto1 = new TransacaoRequisicaoDto { Valor = 100, Tipo = 'c', Descricao = "Depósito" };
            var transacaoDto2 = new TransacaoRequisicaoDto { Valor = 50, Tipo = 'd', Descricao = "Saque" };

            // Realiza algumas transações para gerar o extrato
            await RealizarTransacao(transacaoDto1);
            await RealizarTransacao(transacaoDto2);

            // Act: Execução da operação a ser testada
            var response = await _client.GetAsync("/clientes/1/extrato");

            // Assert: Verificação dos resultados
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = JsonConvert.DeserializeObject<ExtratoRespostaDto>(await response.Content.ReadAsStringAsync());

            // Verifica o saldo total
            responseData.Saldo.Total.Should().Be(50); // Saldo inicial (0) + depósito (100) - saque (50) = 50

            // Verifica o limite
            responseData.Saldo.Limite.Should().Be(100000); // Assumindo um limite inicial de 100000

            //// Verifica as últimas transações
            //responseData.UltimasTransacoes.Should().HaveCount(2); // Deve conter 2 transações

            //// Verifica a primeira transação (Saque)
            //var primeiraTransacao = responseData.UltimasTransacoes[0];
            //primeiraTransacao.Valor.Should().Be(50); // Valor do saque
            //primeiraTransacao.Tipo.Should().Be('d'); // Tipo de transação (débito)
            //primeiraTransacao.Descricao.Should().Be("Saque"); // Descrição da transação

            //// Verifica a segunda transação (Depósito)
            //var segundaTransacao = responseData.UltimasTransacoes[1];
            //segundaTransacao.Valor.Should().Be(100); // Valor do depósito
            //segundaTransacao.Tipo.Should().Be('c'); // Tipo de transação (crédito)
            //segundaTransacao.Descricao.Should().Be("Depósito"); // Descrição da transação
        }

        // Método auxiliar para realizar transações
        private async Task RealizarTransacao(TransacaoRequisicaoDto transacaoDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/clientes/1/transacoes", content);
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("Descrição muito longa que excede o limite")]
        [InlineData("")]
        [InlineData(null)]
        public async Task PostTransacao_DescricaoInvalida_DeveRetornarErro(string descricao)
        {
            var transacaoDto = new { Valor = 1000, Tipo = "c", Descricao = descricao };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task PostTransacao_ValorFracionario_DeveRetornarErro()
        {
            var transacaoDto = new { Valor = 1.2, Tipo = "d", Descricao = "devolve" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.UnprocessableEntity, System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostTransacao_TipoInvalido_DeveRetornarErro()
        {
            var transacaoDto = new { Valor = 1, Tipo = "x", Descricao = "devolve" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task PostTransacao_DescricaoMuitoLonga_DeveRetornarErro()
        {
            var transacaoDto = new { Valor = 1, Tipo = "c", Descricao = "123456789 e mais um pouco" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task PostTransacao_DescricaoVazia_DeveRetornarErro()
        {
            var transacaoDto = new { Valor = 1, Tipo = "c", Descricao = "" };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task PostTransacao_DescricaoNula_DeveRetornarErro()
        {
            var transacaoDto = new { Valor = 1, Tipo = "c", Descricao = (string)null };
            var content = new StringContent(JsonConvert.SerializeObject(transacaoDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/clientes/1/transacoes", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        }
    }
}