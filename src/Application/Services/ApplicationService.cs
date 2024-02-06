using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationEmailService _applicationEmailService;
    private readonly IConfigurationReader _configurationReader;
    private readonly ILogger<ApplicationService> _logger;
    private readonly IOAuthClientCredentialsReader _oAuthClientCredentialsReader;
    private EnvironmentVariables? _environmentVariables;
    private GoogleOAuthClientCredentials? _googleOAuthClientCredentials;

    public ApplicationService(IApplicationEmailService applicationEmailService,
        IConfigurationReader configurationReader, ILogger<ApplicationService> logger,
        IOAuthClientCredentialsReader oAuthClientCredentialsReader)
    {
        _applicationEmailService = applicationEmailService;
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
        _applicationEmailService.GetUnreadEmails(_environmentVariables!);

        await Task.Delay(0);
    }

    // TODO: Run StopAsync on MerrMailWorker
    public Task StopAsync()
    {
        throw new NotImplementedException();
    }
}