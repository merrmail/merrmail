using FluentAssertions;
using Merrsoft.MerrMail.Infrastructure.Options;
using Merrsoft.MerrMail.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.IntegrationTests.ServicesTests;

public class TensorFlowEmailAnalyzerServiceTests
{
    [Fact]
    public void Initialize_ShouldSucceed()
    {
        // Arrange
        var logger = new Logger<TensorFlowEmailAnalyzerService>(new LoggerFactory());
        var aiIntegrationOptions = Options.Create(new EmailAnalyzerOptions { AcceptanceScore = -1.0f });
        var pythonAiIntegrationService = new TensorFlowEmailAnalyzerService(logger, aiIntegrationOptions);

        // Act
        var result = pythonAiIntegrationService.Initialize();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Merrsoft", "Merrsoft", true)]
    [InlineData("Hello", "World", false)]
    public void IsSimilar_ShouldReturnExpectedResult(string first, string second, bool expectedResult)
    {
        // Arrange
        var logger = new Logger<TensorFlowEmailAnalyzerService>(new LoggerFactory());
        var aiIntegrationOptions = Options.Create(new EmailAnalyzerOptions { AcceptanceScore = -0.5f });
        var pythonAiIntegrationService = new TensorFlowEmailAnalyzerService(logger, aiIntegrationOptions);

        // Act
        var result = pythonAiIntegrationService.IsSimilar(first, second);

        // Assert
        result.Should().Be(expectedResult);
    }
}