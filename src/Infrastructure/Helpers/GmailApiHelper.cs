using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Merrsoft.MerrMail.Infrastructure.Helpers;

internal static class GmailApiHelper
{
    public static async Task<GmailService> GetGmailService(string credentialsPath, string accessTokenPath)
    {
        await using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
            new[] { GmailService.Scope.GmailReadonly, GmailService.Scope.GmailModify },
            "user",
            CancellationToken.None,
            new FileDataStore(accessTokenPath, true));

        return new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Gmail API Sample",
        });
    }
}