using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Merrsoft.MerrMail.Infrastructure.Helpers;

public static class GmailApiHelper
{
    public static GmailService GetGmailService(string credentialsPath, string accessTokenPath)
    {
        using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { GmailService.Scope.GmailReadonly },
            "user",
            CancellationToken.None,
            new FileDataStore(accessTokenPath, true)).Result;

        return new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Gmail API Sample",
        });
    } 
}