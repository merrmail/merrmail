using FluentAssertions;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Infrastructure.Options;
using Merrsoft.MerrMail.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

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
    
    [Fact]
    public void GetEmailReply_ShouldGetReply()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TensorFlowEmailAnalyzerService>>();
        var optionsMock = new Mock<IOptions<EmailAnalyzerOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EmailAnalyzerOptions { AcceptanceScore = 0.8f });
        var service = new TensorFlowEmailAnalyzerService(loggerMock.Object, optionsMock.Object);
        var emailContexts = new List<EmailContext>
        {
            new("Subject1", "Response1"),
            new("Hello", "Correct!")
        };
    
        // Act
        var reply = service.GetEmailReply("Hi", emailContexts);
    
        // Assert
        reply.Should().Be("Correct!");
    }
}