namespace MerrMail.Domain.Models;

/// <summary>
/// Represents an email thread with an ID, subject, body, and sender.
/// </summary>
/// <param name="Id">The ID of the email thread.</param>
/// <param name="Subject">The subject of the email thread.</param>
/// <param name="Body">The body of the FIRST email in the thread.</param>
/// <param name="Sender">The sender of the email thread.</param>
public record EmailThread(string Id, string Subject, string Body, string Sender);