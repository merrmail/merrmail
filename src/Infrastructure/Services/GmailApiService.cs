using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Common;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Merrsoft.MerrMail.Infrastructure.Services;

public partial class GmailApiService(
    ILogger<GmailApiService> logger,
    IOptions<EmailApiOptions> emailApiOptions)
    : IEmailApiService
{
    private readonly string _host = emailApiOptions.Value.HostAddress;
    private GmailService? _gmailService;

    public async Task<bool> InitializeAsync()
    {
        try
        {
            logger.LogInformation("Attempting to get Gmail Service...");
            _gmailService = await GetGmailServiceAsync();

            if (_gmailService is null)
            {
                logger.LogError("Gmail Service is null. Initialization failed.");
                return false;
            }

            logger.LogInformation("Gmail Service initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("Error getting or initializing Gmail Service: {message}", ex.Message);
            return false;
        }

        try
        {
            // TODO: Label creations here
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating required labels: {message}", ex.Message);
            return false;
        }

        return true;
    }

    public List<EmailThread> GetThreadsMessage()
    {
        var emails = new List<EmailThread>();
        var threadsResponse = GetThreads();

        if (threadsResponse?.Threads is null)
            return [];

        foreach (var thread in threadsResponse.Threads)
        {
            var threadDetailsResponse = _gmailService!.Users.Threads.Get(_host, thread.Id).Execute();

            if (threadDetailsResponse?.Messages?.Any() is not true)
            {
                logger.LogWarning("Thread ID {threadId} has no messages or failed to retrieve details.", thread.Id);
                continue;
            }

            var firstMessage = threadDetailsResponse.Messages.First();
            var firstEmail = _gmailService!.Users.Messages.Get(_host, firstMessage.Id).Execute();

            if (firstEmail is null)
            {
                logger.LogWarning("Failed to retrieve email for thread ID {threadId}. Message is null.", thread.Id);
                continue;
            }

            var sender = firstEmail.Payload.Headers?.FirstOrDefault(h => h.Name == "From")?.Value ?? "Unknown Sender";
            var parsedEmail = sender.ParseEmail();
            if (parsedEmail.Equals(_host, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Host is the first sender for thread Id {threadId}. Skipping...", thread.Id);
                continue;
            }

            var subject = firstEmail.Payload.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "No Subject";
            var body = firstEmail.Snippet;

            var email = new EmailThread(thread.Id, subject, body, sender);
            logger.LogInformation("Email found: {threadId} | {subject} | {sender} | {body}", email.Id, email.Subject,
                email.Sender, email.Body);
            emails.Add(email);
        }

        return emails;
    }

    public Task Reply(string to)
    {
        throw new NotImplementedException();
    }

    public void MarkAsRead(string messageId)
    {
        var mods = new ModifyMessageRequest
        {
            AddLabelIds = null,
            RemoveLabelIds = new List<string> { "UNREAD" }
        };

        _gmailService!.Users.Messages.Modify(mods, _host, messageId).Execute();
        logger.LogInformation("Marked email as read: {messageId}", messageId);
    }
}