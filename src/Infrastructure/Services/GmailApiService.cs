using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Common;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Domain.Options;
using Merrsoft.MerrMail.Domain.Types;
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
            logger.LogInformation("Creating required labels...");

            if (!CreateLabel("MerrMail: High Priority") || // Red
                !CreateLabel("MerrMail: Low Priority") || // Green
                !CreateLabel("MerrMail: Other")) // Blue
                return false;
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating required labels: {message}", ex.Message);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Retrieves a single EmailThread while filtering out unnecessary threads to optimize API usage.
    /// </summary>
    /// <returns>An EmailThread that has passed all validation checks, or null if no suitable thread is found.</returns>
    public EmailThread? GetEmailThread()
    {
        var threadsResponse = GetThreads();

        if (threadsResponse?.Threads is null)
            return null;

        foreach (var thread in threadsResponse.Threads)
        {
            var threadRequest = _gmailService!.Users.Threads.Get(_host, thread.Id);
            var threadResponse = threadRequest.Execute();

            if (threadResponse?.Messages?.Any() is not true)
            {
                logger.LogWarning("Thread {threadId} has no messages or failed to retrieve details. Skipping...",
                    thread.Id);
                continue;
            }

            var messageRequest = threadResponse.Messages.First();
            var firstMessage = _gmailService!.Users.Messages.Get(_host, messageRequest.Id).Execute();

            if (firstMessage is null)
            {
                logger.LogWarning("Failed to retrieve first email for thread {threadId}. Skipping...", thread.Id);
                continue;
            }

            if (MessageAnalyzed(firstMessage))
            {
                logger.LogInformation("First email for thread {threadId} is already analyzed. Archiving...",
                    thread.Id);
                MoveThread(thread.Id, LabelType.None);
                continue;
            }

            var sender = firstMessage.Payload.Headers?.FirstOrDefault(h => h.Name == "From")?.Value ?? "Unknown Sender";
            var senderAddress = sender.ParseEmail();
            if (senderAddress.Equals(_host, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Host is the first sender for thread {threadId}. Archiving...", thread.Id);
                MoveThread(thread.Id, LabelType.Other);
                continue;
            }

            if (sender.Contains("no") && sender.Contains("reply"))
            {
                logger.LogInformation(
                    "The sender {senderAddress} on thread {threadId} is identified as a 'no-reply'. Archiving...",
                    senderAddress, thread.Id);
                MoveThread(thread.Id, LabelType.Other);
                continue;
            }

            var subject = firstMessage.Payload.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "No Subject";
            var body = firstMessage.Snippet;

            var email = new EmailThread(thread.Id, subject, body, sender);
            logger.LogInformation("Email found: {threadId} | {subject} | {sender} | {body}.", email.Id, email.Subject,
                email.Sender, email.Body);

            return email;
        }

        return null;
    }

    public void MoveThread(string threadId, LabelType addLabel)
    {
        var labelName = GetLabelName(addLabel);
        var labelId = GetLabelId(labelName);

        var modifyThreadRequest = new ModifyThreadRequest
        {
            AddLabelIds = labelId is not null ? new List<string> { labelId } : null,
            RemoveLabelIds = new List<string> { "INBOX" }
        };

        var modifyThreadResponse = _gmailService!.Users.Threads.Modify(modifyThreadRequest, _host, threadId).Execute();
        if (modifyThreadResponse is not null)
            logger.LogInformation("Thread {threadId} moved successfully.", threadId);
        else logger.LogWarning("Failed to move thread {threadId}", threadId);
    }
}