using Merrsoft.MerrMail.Domain.Common;

// ReSharper disable UseVerbatimString
// ReSharper disable StringLiteralTypo

namespace Merrsoft.MerrMail.Domain.UnitTests.CommonTests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("Merrsoft <merrsoft@sample.domain>", "merrsoft@sample.domain")]
    [InlineData("John Doe <john.doe@email.com>", "john.doe@email.com")]
    [InlineData("No Email Address", "")]
    [InlineData("Invalid Email <invalid.email>", "invalid.email")]
    public void ParseEmail_ShouldExtractEmailAddress(string input, string expectedOutput)
    {
        var result = input.ParseEmail();
        result.Should().Be(expectedOutput);
    }
}