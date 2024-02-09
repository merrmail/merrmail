using System.Text;
using Google.Apis.Auth;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Infrastructure.Helpers;

namespace Merrsoft.MerrMail.Infrastructure.Services;

// TODO: Make Gmail service a property
public class GmailApiService : IEmailApiService
{
    private GmailService? _gmailService;
    public List<Email> GetUnreadEmails(EnvironmentVariables env)
    {
        _gmailService = GmailApiHelper.GetGmailService(env.OAuthClientCredentialsPath, env.AccessTokenPath);
        var emails = new List<Email>();

        var listRequest = _gmailService.Users.Messages.List(env.HostAddress);
        listRequest.LabelIds = "INBOX";
        listRequest.IncludeSpamTrash = false;
        listRequest.Q = "is:unread";

        var listResponse = listRequest.Execute();

        if (listResponse.Messages is null)
            return [];

        foreach (var message in listResponse.Messages)
        {
            var messageContentRequest = _gmailService.Users.Messages.Get(env.HostAddress, message.Id);
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
                    body = Base64Helper.GetDecodedString(data);
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
        throw new NotImplementedException();
    }

    public void MarkAsRead(EnvironmentVariables env, string messageId)
    {
        var mods = new ModifyMessageRequest
        {
            AddLabelIds = null,
            RemoveLabelIds = new List<string> { "UNREAD" }
        };

        _gmailService!.Users.Messages.Modify(mods, env.HostAddress, messageId).Execute();
    }
}