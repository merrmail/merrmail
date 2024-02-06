using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class MerrMailService : IMerrMailService
{
    private readonly IConfigurationReader _configurationReader;
    private readonly ILogger<MerrMailService> _logger;
    private EnvironmentVariables? _environmentVariables;

    public MerrMailService(IConfigurationReader configurationReader, ILogger<MerrMailService> logger)
    {
        _configurationReader = configurationReader;
        _logger = logger;
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
        
        // TODO: Validate database connection

        return false;
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