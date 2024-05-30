using System.Net;
using System.Net.Mail;
using MerrMail.Application.Contracts;
using MerrMail.Domain.Common;
using MerrMail.Domain.Models;
using MerrMail.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MerrMail.Infrastructure.Services;

/// <summary>
/// Email reply service implementation for replying to email threads using SMTP.
/// </summary>
/// <param name="logger">The logger instance.</param>
/// <param name="replyContentOptions">The options for email reply content.</param>
/// <param name="emailApiOptions">The options for email API configuration.</param>
public class SmtpReplyService(
    ILogger<SmtpReplyService> logger,
    IOptions<EmailReplyOptions> replyContentOptions,
    IOptions<EmailApiOptions> emailApiOptions) : IEmailReplyService
{
    private readonly string _host = emailApiOptions.Value.HostAddress;
    private readonly string _password = emailApiOptions.Value.HostPassword;
    private readonly string _header = replyContentOptions.Value.Header;
    private readonly string _introduction = replyContentOptions.Value.Introduction;
    private readonly string _conclusion = replyContentOptions.Value.Conclusion;
    private readonly string _closing = replyContentOptions.Value.Closing;
    private readonly string _signature = replyContentOptions.Value.Signature;

    /// <summary>
    /// Adds a reply to an email thread, sent by the host.
    /// </summary>
    /// <param name="emailThread">The email thread to reply to.</param>
    /// <param name="message">The message from the host.</param>
    public void ReplyThread(EmailThread emailThread, string message)
    {
        var emailBody = new EmailReplyBuilder()
            .SetHeader(_header)
            .SetIntroduction(_introduction)
            .SetMessage(message)
            .SetConclusion(_conclusion)
            .SetClosing(_closing)
            .SetSignature(_signature)
            .Build();

        const int gmailSmtpPort = 587;
        var smtpClient = new SmtpClient("smtp.gmail.com", gmailSmtpPort);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(_host, _password);

        using var mailMessage = new MailMessage(_host, emailThread.Sender);
        mailMessage.Subject = "Re: " + emailThread.Subject;
        mailMessage.Headers.Add("In-Reply-To", emailThread.Id);
        mailMessage.Headers.Add("References", emailThread.Id);
        mailMessage.Body = emailBody;
        mailMessage.IsBodyHtml = true;

        try
        {
            smtpClient.Send(mailMessage);
            logger.LogInformation("Replied to thread {threadId}.", emailThread.Id);
        }
        catch (Exception ex)
        {
            logger.LogError("Error replying to thread {threadId}: {message}", emailThread.Id, ex.Message);
        }
    }
}