using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Common;
using Merrsoft.MerrMail.Domain.Enums;
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
            logger.LogInformation("Creating required labels...");

            CreateLabel("MerrMail: High Priority"); // Red
            CreateLabel("MerrMail: Low Priority"); // Green
            CreateLabel("MerrMail: Other"); // Blue
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating required labels: {message}", ex.Message);
            return false;
        }

        return true;
    }

    public void LabelThread(LabelType labelType, EmailThread emailThread)
    {
        var threadDetailsRequest = _gmailService!.Users.Threads.Get(_host, emailThread.Id);
        var threadDetailsResponse = threadDetailsRequest.Execute();

        if (threadDetailsResponse?.Messages?.Any() is not true)
        {
            logger.LogWarning("Thread ID {threadId} has no messages or failed to retrieve details.", emailThread.Id);
            return;
        }

        var labelId = GetLabelId(GetLabelName(labelType));

        var modifyThreadRequest = new ModifyThreadRequest { AddLabelIds = new List<string> { labelId! } };
        _gmailService!.Users.Threads.Modify(modifyThreadRequest, _host, emailThread.Id).Execute();
    }

    public bool ThreadShouldAnalyze(EmailThread emailThread)
    {
        var threadDetailsRequest = _gmailService!.Users.Threads.Get(_host, emailThread.Id);
        var threadDetailsResponse = threadDetailsRequest.Execute();

        if (threadDetailsResponse?.Messages?.Any() is not true)
        {
            logger.LogWarning("Thread {threadId} has no messages or failed to retrieve details.", emailThread.Id);
            return false;
        }

        if (ThreadAnalyzed(threadDetailsResponse))
        {
            logger.LogInformation("Thread {threadId} is already analyzed. Skipping", emailThread.Id);
            return false;
        }

        if (!emailThread.Host) return true;
        logger.LogInformation("Host is the creator of the thread {threadId}.", emailThread.Id);
        return false;
    }

    public void ArchiveThread(EmailThread emailThread)
    {
        if (string.IsNullOrEmpty(emailThread.Id))
            return;

        List<string>? labelIds = null;
        if (emailThread.Host)
            labelIds = new List<string> { GetLabelId(GetLabelName(LabelType.Other))! };

        var modifyThreadRequest = new ModifyThreadRequest
        {
            AddLabelIds = labelIds,
            RemoveLabelIds = new List<string> { "INBOX" }
        };

        _gmailService!.Users.Threads.Modify(modifyThreadRequest, _host, emailThread.Id).Execute();
        logger.LogInformation("Thread {threadId} archived successfully.", emailThread);
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
            var host = false;
            if (parsedEmail.Equals(_host, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Host is the first sender for thread Id {threadId}. Marking as host.", thread.Id);
                host = true;
            }

            var subject = firstEmail.Payload.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "No Subject";
            var body = firstEmail.Snippet;

            var email = new EmailThread(thread.Id, subject, body, sender, host);
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
}