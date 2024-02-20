using System.Text.RegularExpressions;
// ReSharper disable InvalidXmlDocComment

namespace Merrsoft.MerrMail.Domain.Common;

public static partial class StringExtensions
{
    // This should also replaces escaping characters
    public static string ToDecodedString(this string data)
    {
        // TODO: Implement decoder
        return data;
    }
    
    public static string ToEncodedString(this string data)
    {
        return data;
    }

    /// <summary>
    /// Parses the sender information and returns the email address.
    /// </summary>
    /// <param name="data">The input string containing sender information. Example: "Merrsoft <merrsoft@sample.domain>".</param>
    /// <returns>The extracted email address, or an empty string if not found. Example: "merrsoft@sample.domain".</returns>
    public static string ParseEmail(this string data)
    {
        // Using a regular expression to match and extract the email address
        var match = EmailParserRegex().Match(data);

        // Returning the extracted email address, or an empty string if not found
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    [GeneratedRegex("<([^>]+)>")]
    private static partial Regex EmailParserRegex();
}