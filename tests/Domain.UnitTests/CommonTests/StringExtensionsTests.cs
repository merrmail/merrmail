using FluentAssertions;
using Merrsoft.MerrMail.Domain.Common;
// ReSharper disable UseVerbatimString
// ReSharper disable StringLiteralTypo

namespace Domain.UnitTests.CommonTests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("Hello, World!", "SGVsbG8sIFdvcmxkIQ==")]
    [InlineData("12345", "MTIzNDU=")]
    [InlineData("", "")]
    [InlineData("Test Message", "VGVzdCBNZXNzYWdl")]
    [InlineData("Special Characters: !@#$%^&*()_+", "U3BlY2lhbCBDaGFyYWN0ZXJzOiAhQCMkJV4mKigpXykr")]
    [InlineData("Line1\nLine2", "TGluZTEKTGluZTI=")]
    [InlineData("Carriage\rReturn", "Q2FycmlhZ2VcclJldHVybg==")]
    [InlineData("Double\\Backslash", "RG91YmxlXFxCYWNrc2xhc2g=")] 
    [InlineData("With\\Escaping\\Characters", "V2l0aFxEYXJrZXJnXFxDaGFyYWN0ZXJz")]
    public void ToEncodedString_ShouldEncodeString(string input, string expected)
    {
        // Act
        var encodedString = input.ToEncodedString();

        // Assert
        encodedString.Should().Be(expected);
    }

    [Theory]
    [InlineData("SGVsbG8sIFdvcmxkIQ==", "Hello, World!")]
    [InlineData("MTIzNDU=", "12345")]
    [InlineData("", "")]
    [InlineData("VGVzdCBNZXNzYWdl", "Test Message")]
    [InlineData("U3BlY2lhbCBDaGFyYWN0ZXJzOiAhQCMkJV4mKigpXykr", "Special Characters: !@#$%^&*()_+")]
    [InlineData("TGluZTEKTGluZTI=", "Line1 Line2")] 
    [InlineData("Q2FycmlhZ2UgXHJSZXR1cm4=", "Carriage Return")] 
    [InlineData("RG91YmxlXFxCYWNrc2xhc2g=", "Double Backslash")]
    [InlineData("V2l0aFxEYXJrZXJnXFxDaGFyYWN0ZXJz", "With Escaping Characters")]
    public void ToDecodedString_ShouldDecodeStringAndReplaceEscapes(string input, string expected)
    {
        // Act
        var decodedString = input.ToDecodedString();

        // Assert
        decodedString.Should().Be(expected);
    }
}