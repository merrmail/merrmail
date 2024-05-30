namespace MerrMail.Domain.Models;

/// <summary>
/// Represents an email context with a subject and a response.
/// </summary>
/// <param name="Subject">The subject of the email context.</param>
/// <param name="Response">The response associated with the email context.</param>
public record EmailContext(string Subject, string Response);