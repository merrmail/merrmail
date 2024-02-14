using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Infrastructure.Readers;

public class EnvConfigurationReader : IConfigurationReader
{
    public EnvironmentVariables ReadConfiguration()
    {
        DotNetEnv.Env.TraversePath().Load();

        var env = new EnvironmentVariables
        {
            OAuthClientCredentialsPath = DotNetEnv.Env.GetString("OAUTH_CLIENT_CREDENTIALS_PATH"),
            AccessTokenPath = DotNetEnv.Env.GetString("ACCESS_TOKEN_PATH"),
            DatabaseConnection = DotNetEnv.Env.GetString("DATABASE_CONNECTION"),
            HostAddress = DotNetEnv.Env.GetString("HOST_ADDRESS"),
        };

        return env;
    }
}