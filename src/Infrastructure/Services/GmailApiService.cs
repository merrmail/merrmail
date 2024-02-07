using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Infrastructure.Helpers;

namespace Merrsoft.MerrMail.Infrastructure.Services;

// TODO: Make Gmail service a property
public class GmailApiService : IEmailApiService
{
    public List<Email> GetUnreadEmails(EnvironmentVariables env)
    {
        var service = GmailApiHelper.GetGmailService(env.OAuthClientCredentialsPath, env.AccessTokenPath);
        var emails = new List<Email>();

        var listRequest = service.Users.Messages.List(env.HostAddress);
        listRequest.LabelIds = "INBOX";
        listRequest.IncludeSpamTrash = false;
        listRequest.Q = "is:unread";

        var listResponse = listRequest.Execute();

        if (listResponse.Messages is null)
            return [];

        foreach (var message in listResponse.Messages)
        {
            var messageContentRequest = service.Users.Messages.Get(env.HostAddress, message.Id);
            var messageContent = messageContentRequest.Execute();

            if (messageContent is not null)
            {
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

                // TODO: Read nested messages
                if (messageContent.Payload.Parts is null && messageContent.Payload.Body is not null)
                    body = messageContent.Payload.Body.Data;

                // TODO: Decode the body
                var email = new Email(from, to, body, mailDateTime, attachments, id);
                emails.Add(email);
            }
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

        var service = GmailApiHelper.GetGmailService(env.OAuthClientCredentialsPath, env.AccessTokenPath);
        service.Users.Messages.Modify(mods, env.HostAddress, messageId).Execute();
    }
}