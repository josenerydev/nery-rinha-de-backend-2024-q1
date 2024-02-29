using Account.Api.Services;

using FluentAssertions;

namespace Account.Tests
{
    public class TransacaoValidacaoServiceTests
    {
        [Fact]
        public void ValidarValor_ValorPositivo_DeveRetornarValido()
        {
            // Arrange
            var validacaoService = new TransacaoValidacaoService();
            int valorTeste = 100; // Valor positivo para o teste

            // Act
            var resultado = validacaoService.ValidarValor(valorTeste);

            // Assert
            resultado.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidarValor_ValorNegativo_DeveRetornarInvalidoComMensagem()
        {
            // Arrange
            var validacaoService = new TransacaoValidacaoService();
            int valorTeste = -100; // Valor negativo para o teste

            // Act
            var resultado = validacaoService.ValidarValor(valorTeste);

            // Assert
            resultado.IsValid.Should().BeFalse();
            resultado.ErrorMessage.Should().Be("Valor deve ser positivo e inteiro.");
        }

        [Theory]
        [InlineData('c', true)]  // 'c' é um tipo válido
        [InlineData('d', true)]  // 'd' é um tipo válido
        [InlineData('x', false)] // 'x' é um tipo inválido
        public void ValidarTipo_TipoTransacao_DeveSerValidoOuInvalido(char tipo, bool esperadoValido)
        {
            // Arrange
            var validacaoService = new TransacaoValidacaoService();

            // Act
            var resultado = validacaoService.ValidarTipo(tipo);

            // Assert
            resultado.IsValid.Should().Be(esperadoValido);
        }

        [Theory]
        [InlineData("Compra", true)]  // Descrição válida
        [InlineData("", false)]       // Descrição inválida (vazia)
        [InlineData(null, false)]       // Descrição inválida (null)
        [InlineData("   ", false)]       // Descrição inválida (espaço)
        [InlineData("12345678901", false)] // Descrição inválida (mais de 10 caracteres)
        public void ValidarDescricao_DescricaoTransacao_DeveSerValidaOuInvalida(string descricao, bool esperadoValido)
        {
            // Arrange
            var validacaoService = new TransacaoValidacaoService();

            // Act
            var resultado = validacaoService.ValidarDescricao(descricao);

            // Assert
            resultado.IsValid.Should().Be(esperadoValido);
        }
    }
}