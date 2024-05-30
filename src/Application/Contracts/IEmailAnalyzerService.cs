using MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;

/// <summary>
/// Represents a contract for analyzing and generating email replies.
/// </summary>
public interface IEmailAnalyzerService
{
    /// <summary>
    /// Initializes the email analyzer.
    /// </summary>
    /// <returns>True if initialization is successful, otherwise false.</returns>
    bool Initialize();
    
    /// <summary>
    /// Retrieves the best matching email reply based on email context and similarity scores.
    /// </summary>
    /// <param name="emailThread">The email thread to analyze.</param>
    /// <param name="emailContexts">The email contexts to consider in the analysis.</param>
    /// <returns>The appropriate reply, or null if no reply is found.</returns>
    string? GetEmailReply(EmailThread emailThread, IEnumerable<EmailContext> emailContexts);
}