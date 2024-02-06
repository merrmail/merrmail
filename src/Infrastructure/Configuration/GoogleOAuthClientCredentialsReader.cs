using Merrsoft.MerrMail.Application.Interfaces;
using Merrsoft.MerrMail.Domain.Models;
using Newtonsoft.Json;

namespace Merrsoft.MerrMail.Infrastructure.Configuration;

public class GoogleOAuthClientCredentialsReader : IOAuthClientCredentialsReader
{
    public async Task<GoogleOAuthClientCredentials?> ReadCredentialsAsync(string id)
    {
        var json = await File.ReadAllTextAsync(id);

        var credentials = JsonConvert.DeserializeObject<GoogleOAuthClientCredentials>(json);

        return credentials;
    }
}