using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Contracts;

namespace Merrsoft.MerrMail.Infrastructure.Readers;

public class EnvConfigurationReader : IConfigurationReader
{
    private readonly IConfigurationSettings _configurationSettings;

    public EnvConfigurationReader(IConfigurationSettings configurationSettings)
    {
        _configurationSettings = configurationSettings;
    }

    public void ReadConfiguration()
    {
        DotNetEnv.Env.TraversePath().Load();

        _configurationSettings.OAuthClientCredentialsPath = DotNetEnv.Env.GetString("OAUTH_CLIENT_CREDENTIALS_PATH");
        _configurationSettings.AccessTokenPath = DotNetEnv.Env.GetString("ACCESS_TOKEN_PATH");
        _configurationSettings.DatabaseConnection = DotNetEnv.Env.GetString("DATABASE_CONNECTION");
        _configurationSettings.HostAddress = DotNetEnv.Env.GetString("HOST_ADDRESS");
    }
}