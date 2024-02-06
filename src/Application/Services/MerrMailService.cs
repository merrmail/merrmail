using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Services;

public class MerrMailService : IMerrMailService
{
    private readonly IConfigurationReader _configurationReader;
    private EnvironmentVariables _environmentVariables;

    public MerrMailService(IConfigurationReader configurationReader)
    {
        _configurationReader = configurationReader;
    }
    
    public async Task StartAsync()
    {
        _environmentVariables = _configurationReader.ReadConfiguration();
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