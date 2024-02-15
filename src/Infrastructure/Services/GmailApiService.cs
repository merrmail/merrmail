using Google.Apis.Auth;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Common;
using Merrsoft.MerrMail.Domain.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Infrastructure.Services;

// TODO: Make Gmail service a property
public class GmailApiService(ILogger<GmailApiService> logger, IConfigurationSettings configurationSettings)
    : IEmailApiService
{
    private GmailService? _gmailService;

    private void Initialize()
    {
        _gmailService ??= GmailApiHelper.GetGmailService(
            configurationSettings.OAuthClientCredentialsPath,
            configurationSettings.AccessTokenPath);
    }

    public List<Email> GetUnreadEmails()
    {
        Initialize();
        
        var emails = new List<Email>();

        var listRequest = _gmailService!.Users.Messages.List(configurationSettings.HostAddress);
        listRequest.LabelIds = "INBOX";
        listRequest.IncludeSpamTrash = false;
        listRequest.Q = "is:unread";

        var listResponse = listRequest.Execute();

        if (listResponse.Messages is null)
            return [];

        foreach (var message in listResponse.Messages)
        {
            var messageContentRequest =
                _gmailService.Users.Messages.Get(configurationSettings.HostAddress, message.Id);
            var messageContent = messageContentRequest.Execute();

            if (messageContent is null) continue;

            // TODO: Remove unnecessary variables
            // TODO: Remove unnecessary Email properties
            var from = string.Empty;
            var to = string.Empty;
            var body = string.Empty;
            var subject = string.Empty;
            var date = string.Empty;
            var mailDateTime = DateTime.Now;
            var attachments = new List<string>();
            var id = message.Id;

            foreach (var messageParts in messageContent.Payload.Headers)
            {
                switch (messageParts.Name)
                {
                    case "From":
                        from = messageParts.Value;
                        break;
                    case "Date":
                        date = messageParts.Value;
                        break;
                    case "Subject":
                        subject = messageParts.Value;
                        break;
                }
            }

            if (messageContent.Payload.Parts is not null && messageContent.Payload.Parts.Count > 0)
            {
                var firstPart = messageContent.Payload.Parts[0];

                if (firstPart.Body?.Data != null)
                {
                    var data = firstPart.Body.Data;
                    body = data.ToDecodedString();
                }
            }

            // TODO: Decode the body
            var email = new Email(from, to, body, mailDateTime, attachments, id);
            emails.Add(email);
        }

        return emails;
    }

    public Task Reply(string to)
    {
        Initialize();
        throw new NotImplementedException();
    }

    public void MarkAsRead(string messageId)
    {
        Initialize();
        
        var mods = new ModifyMessageRequest
        {
            AddLabelIds = null,
            RemoveLabelIds = new List<string> { "UNREAD" }
        };

        _gmailService!.Users.Messages.Modify(mods, configurationSettings.HostAddress, messageId).Execute();
        logger.LogInformation("Marked email as read: {messageId}", messageId);
    }
}