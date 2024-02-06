using System.Text.Json;
using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Common;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class MerrMailService : IMerrMailService
{
    private readonly IConfigurationReader _configurationReader;
    private readonly ILogger<MerrMailService> _logger;
    private readonly IOAuthClientCredentialsReader _oAuthClientCredentialsReader;
    private EnvironmentVariables? _environmentVariables;
    private GoogleOAuthClientCredentials? _googleOAuthClientCredentials;

    public MerrMailService(IConfigurationReader configurationReader, ILogger<MerrMailService> logger,
        IOAuthClientCredentialsReader oAuthClientCredentialsReader)
    {
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

    public Task RunAsync()
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        throw new NotImplementedException();
    }
}