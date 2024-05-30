using MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;

public interface IEmailAnalyzerService
{
    /// <summary>
    /// Initializes the email analyzer.
    /// </summary>
    /// <returns>True if initialization is successful, otherwise false.</returns>
    bool Initialize();
    
    /// <summary>
    /// Gets a reply for the specified email thread from the email analyzer.
    /// </summary>
    /// <param name="emailThread">The email thread to analyze.</param>
    /// <param name="emailContexts">The email contexts to consider in the analysis.</param>
    /// <returns>The appropriate reply, or null if no reply is found.</returns>
    string? GetEmailReply(EmailThread emailThread, IEnumerable<EmailContext> emailContexts);
}