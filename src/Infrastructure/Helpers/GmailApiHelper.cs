using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

// ReSharper disable once CheckNamespace
namespace Merrsoft.MerrMail.Infrastructure.Services;

public partial class GmailApiService
{
    private readonly string _credentialsPath = emailApiOptions.Value.OAuthClientCredentialsFilePath;
    private readonly string _tokenPath = emailApiOptions.Value.AccessTokenDirectoryPath;
    
    private async Task<GmailService> GetGmailServiceAsync()
    {
        await using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
            new[] { GmailService.Scope.GmailReadonly, GmailService.Scope.GmailModify },
            "user",
            CancellationToken.None,
            new FileDataStore(_tokenPath, true));

        return new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Gmail API Sample",
        });
    }

    private ListThreadsResponse? GetThreads()
    {
        try
        {
            var threadsRequest = _gmailService!.Users.Threads.List(_host);
            threadsRequest.LabelIds = "INBOX";
            threadsRequest.IncludeSpamTrash = false;
            var threadsResponse = threadsRequest.Execute();

            return threadsResponse;
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }
}