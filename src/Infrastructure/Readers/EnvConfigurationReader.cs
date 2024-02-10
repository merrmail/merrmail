using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Infrastructure.Readers;

public class EnvConfigurationReader : IConfigurationReader
{
    public EnvironmentVariables ReadConfiguration()
    {
        DotNetEnv.Env.TraversePath().Load();

        var oAuthClientCredentialsPath = DotNetEnv.Env.GetString("OAUTH_CLIENT_CREDENTIALS_PATH");
        var accessTokenPath = DotNetEnv.Env.GetString("ACCESS_TOKEN_PATH");
        var databaseConnection = DotNetEnv.Env.GetString("DATABASE_CONNECTION");
        var hostAddress = DotNetEnv.Env.GetString("HOST_ADDRESS");

        var env = new EnvironmentVariables(oAuthClientCredentialsPath, accessTokenPath, databaseConnection,
            hostAddress);

        return env;
    }
}