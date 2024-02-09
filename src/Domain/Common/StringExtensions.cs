namespace Merrsoft.MerrMail.Domain.Common;

public static class StringExtensions
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
}