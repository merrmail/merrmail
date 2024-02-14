using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class ApplicationService(
    IConfigurationSettings configurationSettings,
    IEmailApiService emailApiService,
    IConfigurationReader configurationReader,
    ILogger<ApplicationService> logger,
    IOAuthClientCredentialsReader oAuthClientCredentialsReader)
    : IApplicationService
{
    private GoogleOAuthClientCredentials? _googleOAuthClientCredentials;

    public async Task<bool> CanStartAsync()
    {
        logger.LogInformation("Reading configuration...");
        configurationReader.ReadConfiguration();

        if (!File.Exists(configurationSettings.OAuthClientCredentialsPath))
        {
            logger.LogCritical("OAuth Client Credentials Path does not exists");

            return false;
        }

        logger.LogInformation("Reading credentials...");
        var credentialsPath = configurationSettings.OAuthClientCredentialsPath;
        _googleOAuthClientCredentials = await oAuthClientCredentialsReader.ReadCredentialsAsync(credentialsPath);

        // TODO: Validate credentials
        // TODO: Validate database connection

        return true;
    }

    public async Task RunAsync()
    {
        // Environment variables we're already checked before calling RunAsync() so it can't be null
        var emails = emailApiService.GetUnreadEmails();

        // TODO: Mark email as read
        foreach (var email in emails)
        {
            logger.LogInformation("Email found, (Message Id: {emailId})", email.MessageId);
            // _emailApiService.MarkAsRead(email.MessageId);
        }

        // TODO: Check if an email is a concern
        // TODO: Compare email to database
        // TODO: Label emails
        // TODO: Reply to emails

        await Task.Delay(0);
    }

    // TODO: Run StopAsync on MerrMailWorker
    public Task StopAsync()
    {
        throw new NotImplementedException();
    }
}