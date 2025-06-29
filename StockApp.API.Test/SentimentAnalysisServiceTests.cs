using StockApp.Application.Services;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.API.Test
{
    public class SentimentAnalysisServiceTests
    {
        private readonly SentimentAnalysisService _sentimentAnalysisService;

        public SentimentAnalysisServiceTests()
        {
            _sentimentAnalysisService = new SentimentAnalysisService();
        }

        [Theory]
        [InlineData("Excelente produto, estou muito satisfeito com a compra!", "Positivo")]
        [InlineData("Produto bom, atendeu minhas expectativas.", "Positivo")]
        [InlineData("Produto ok, mas poderia ser melhor.", "Neutro")]
        [InlineData("Não gostei do produto, não recomendo.", "Negativo")]
        [InlineData("Péssimo produto, desperdicei meu dinheiro.", "Negativo")]
        public void AnalyzeSentiment_ShouldReturnCorrectSentiment(string review, string expectedSentiment)
        {
            // Act
            var result = _sentimentAnalysisService.AnalyzeSentiment(review);

            // Assert
            Assert.Equal(expectedSentiment, result);
        }

        [Theory]
        [InlineData("Excelente produto, estou muito satisfeito com a compra!", "Positivo")]
        [InlineData("Produto bom, atendeu minhas expectativas.", "Positivo")]
        [InlineData("Produto ok, mas poderia ser melhor.", "Neutro")]
        [InlineData("Não gostei do produto, não recomendo.", "Negativo")]
        [InlineData("Péssimo produto, desperdicei meu dinheiro.", "Negativo")]
        public async Task AnalyzeSentimentAsync_ShouldReturnCorrectSentiment(string review, string expectedSentiment)
        {
            // Act
            var result = await _sentimentAnalysisService.AnalyzeSentimentAsync(review);

            // Assert
            Assert.Equal(expectedSentiment, result);
        }

        [Theory]
        [InlineData("O produto tem boa qualidade, mas é muito caro.", "Neutro")]
        [InlineData("Gostei do produto, mas veio com um pequeno defeito.", "Neutro")]
        [InlineData("Produto bom, mas a entrega foi péssima.", "Neutro")]
        public void AnalyzeSentiment_MixedSentiment_ShouldReturnNeutro(string review, string expectedSentiment)
        {
            // Act
            var result = _sentimentAnalysisService.AnalyzeSentiment(review);

            // Assert
            Assert.Equal(expectedSentiment, result);
        }

        [Theory]
        [InlineData("O produto tem boa qualidade, mas é muito caro.", "Neutro")]
        [InlineData("Gostei do produto, mas veio com um pequeno defeito.", "Neutro")]
        [InlineData("Produto bom, mas a entrega foi péssima.", "Neutro")]
        public async Task AnalyzeSentimentAsync_MixedSentiment_ShouldReturnNeutro(string review, string expectedSentiment)
        {
            // Act
            var result = await _sentimentAnalysisService.AnalyzeSentimentAsync(review);

            // Assert
            Assert.Equal(expectedSentiment, result);
        }

        [Fact]
        public void AnalyzeSentiment_EmptyString_ShouldReturnNeutro()
        {
            // Arrange
            var review = "";

            // Act
            var result = _sentimentAnalysisService.AnalyzeSentiment(review);

            // Assert
            Assert.Equal("Neutro", result);
        }

        [Fact]
        public async Task AnalyzeSentimentAsync_EmptyString_ShouldReturnNeutro()
        {
            // Arrange
            var review = "";

            // Act
            var result = await _sentimentAnalysisService.AnalyzeSentimentAsync(review);

            // Assert
            Assert.Equal("Neutro", result);
        }

        [Fact]
        public void AnalyzeSentiment_NullString_ShouldReturnNeutro()
        {
            // Arrange
            string review = null;

            // Act
            var result = _sentimentAnalysisService.AnalyzeSentiment(review);

            // Assert
            Assert.Equal("Neutro", result);
        }

        [Fact]
        public async Task AnalyzeSentimentAsync_NullString_ShouldReturnNeutro()
        {
            // Arrange
            string review = null;

            // Act
            var result = await _sentimentAnalysisService.AnalyzeSentimentAsync(review);

            // Assert
            Assert.Equal("Neutro", result);
        }
    }
}