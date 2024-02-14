using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Contracts;

namespace Merrsoft.MerrMail.Infrastructure.Readers;

public class EnvConfigurationReader(IConfigurationSettings configurationSettings) : IConfigurationReader
{
    public void ReadConfiguration()
    {
        DotNetEnv.Env.TraversePath().Load();

        configurationSettings.OAuthClientCredentialsPath = DotNetEnv.Env.GetString("OAUTH_CLIENT_CREDENTIALS_PATH");
        configurationSettings.AccessTokenPath = DotNetEnv.Env.GetString("ACCESS_TOKEN_PATH");
        configurationSettings.DatabaseConnection = DotNetEnv.Env.GetString("DATABASE_CONNECTION");
        configurationSettings.HostAddress = DotNetEnv.Env.GetString("HOST_ADDRESS");
    }
}