namespace Merrsoft.MerrMail.Domain.Models;

public record Email(
    string From,
    string To,
    string Body,
    DateTime MailDateTime,
    List<string> Attachments,
    string MessageId);