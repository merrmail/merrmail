using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IEmailApiService _emailApiService;
    private readonly IConfigurationReader _configurationReader;
    private readonly ILogger<ApplicationService> _logger;
    private readonly IOAuthClientCredentialsReader _oAuthClientCredentialsReader;
    private EnvironmentVariables _environmentVariables;
    private GoogleOAuthClientCredentials? _googleOAuthClientCredentials;

    public ApplicationService(IEmailApiService emailApiService,
        IConfigurationReader configurationReader, ILogger<ApplicationService> logger,
        IOAuthClientCredentialsReader oAuthClientCredentialsReader)
    {
        _emailApiService = emailApiService;
        _configurationReader = configurationReader;
        _logger = logger;
        _oAuthClientCredentialsReader = oAuthClientCredentialsReader;
    }

    public async Task<bool> CanStartAsync()
    {
        _logger.LogInformation("Reading configuration...");
        _environmentVariables = _configurationReader.ReadConfiguration();

        if (!File.Exists(_environmentVariables.OAuthClientCredentialsPath))
        {
            _logger.LogCritical("OAuth Client Credentials Path does not exists");

            return false;
        }

        _logger.LogInformation("Reading credentials...");
        var credentialsPath = _environmentVariables.OAuthClientCredentialsPath;
        _googleOAuthClientCredentials = await _oAuthClientCredentialsReader.ReadCredentialsAsync(credentialsPath);

        // TODO: Validate credentials
        // TODO: Validate database connection

        return true;
    }

    public async Task RunAsync()
    {
        // Environment variables we're already checked before calling RunAsync() so it can't be null
        var emails = _emailApiService.GetUnreadEmails(_environmentVariables);
        
        // TODO: Mark email as read
        foreach (var email in emails)
        {
            _logger.LogInformation("Email found, (Message Id: {emailId})", email.MessageId);
            // _emailApiService.MarkAsRead(_environmentVariables, email.MessageId);
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