using HtmlAgilityPack;
using Merrsoft.MerrMail.Domain.Common;

namespace Merrsoft.MerrMail.Domain.UnitTests.CommonTests;

public class EmailReplyBuilderTests
{
    [Fact]
    public void Build_ShouldGenerateValidHtml()
    {
        var emailReplyBuilder = new EmailReplyBuilder()
            .SetHeader("Test Header")
            .SetIntroduction("Test Introduction")
            .SetMessage("Test Message")
            .SetConclusion("Test Conclusion")
            .SetClosing("Test Closing")
            .SetSignature("Test Signature");

        var generatedHtml = emailReplyBuilder.Build();

        generatedHtml.Should().Contain("MerrMail");
        
        var act = () => new HtmlDocument().LoadHtml(generatedHtml);
        act.Should().NotThrow();
    }
}