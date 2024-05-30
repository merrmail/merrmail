using MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;

/// <summary>
/// Represents a contract for replying to email threads.
/// </summary>
public interface IEmailReplyService
{
    /// <summary>
    /// Adds a reply to an email thread, sent by the host.
    /// </summary>
    /// <param name="emailThread">The email thread to reply to.</param>
    /// <param name="message">The message from the host.</param>
    void ReplyThread(EmailThread emailThread, string message);
}