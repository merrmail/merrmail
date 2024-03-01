using HtmlAgilityPack;
using Merrsoft.MerrMail.Domain.Common;

namespace Merrsoft.MerrMail.Domain.UnitTests.CommonTests;

public class EmailReplyBuilderTests
{
    [Fact]
    public void Build_ShouldGenerateValidHtml()
    {
        // Arrange
        var emailReplyBuilder = new EmailReplyBuilder()
            .SetHeader("Test Header")
            .SetIntroduction("Test Introduction")
            .SetMessage("Test Message")
            .SetConclusion("Test Conclusion")
            .SetClosing("Test Closing")
            .SetSignature("Test Signature");

        // Act
        var generatedHtml = emailReplyBuilder.Build();

        // Assert
        var act = () => new HtmlDocument().LoadHtml(generatedHtml);
        act.Should().NotThrow();
        
        // Providing credit is much appreciated
        generatedHtml.Should().Contain("MerrMail");
    }
}