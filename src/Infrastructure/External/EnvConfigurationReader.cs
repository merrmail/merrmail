using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Infrastructure.External;

public class EnvConfigurationReader(
    ILogger<EnvConfigurationReader> logger,
    IConfigurationSettings configurationSettings) : IConfigurationReader
{
    public void ReadConfiguration()
    {
        logger.LogInformation("Reading environment variables...");
        DotNetEnv.Env.TraversePath().Load();

        var oAuthClientCredentialsPath = DotNetEnv.Env.GetString("OAUTH_CLIENT_CREDENTIALS_PATH");
        var accessTokenPath = DotNetEnv.Env.GetString("ACCESS_TOKEN_PATH");
        var databaseConnection = DotNetEnv.Env.GetString("DATABASE_CONNECTION");
        var hostAddress = DotNetEnv.Env.GetString("HOST_ADDRESS");

        configurationSettings.OAuthClientCredentialsPath = oAuthClientCredentialsPath;
        configurationSettings.AccessTokenPath = accessTokenPath;
        configurationSettings.DatabaseConnection = databaseConnection;
        configurationSettings.HostAddress = hostAddress;

        typeof(IConfigurationSettings).GetProperties().ToList().ForEach(property =>
        {
            var value = property.GetValue(configurationSettings);
            logger.Log(value is null
                    ? LogLevel.Critical
                    : LogLevel.Information,
                value is null
                    ? "{propertyName} is null"
                    : "{propertyName} is set to: {value}", property.Name, value);
        });
    }
}