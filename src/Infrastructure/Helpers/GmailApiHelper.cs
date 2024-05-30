using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MerrMail.Domain.Types;
using Microsoft.Extensions.Logging;

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
            ApplicationName = "MerrMail Gmail API",
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

    private bool CreateLabel(string labelName)
    {
        var labelsRequest = _gmailService!.Users.Labels.List(_host);
        var labelsResponse = labelsRequest!.Execute();

        if (labelsResponse?.Labels?.Any(label => label.Name == labelName) is true)
        {
            logger.LogInformation("Label: ({labelName}) already exists. Skipping...", labelName);
            return true;
        }

        string backgroundColor, textColor;

        switch (labelName)
        {
            case "MerrMail: High Priority":
                backgroundColor = "#fb4c2f"; // Red
                textColor = "#ffffff"; // White
                break;
            case "MerrMail: Low Priority":
                backgroundColor = "#16a766"; // Green
                textColor = "#ffffff"; // White
                break;
            case "MerrMail: Other":
                backgroundColor = "#285bac"; // Blue
                textColor = "#ffffff"; // White
                break;
            default:
                backgroundColor = "#000000"; // Black
                textColor = "#ffffff"; // White
                break;
        }

        var label = new Label
        {
            Name = labelName,
            Color = new LabelColor
            {
                BackgroundColor = backgroundColor,
                TextColor = textColor
            }
        };

        var createLabelRequest = _gmailService!.Users.Labels.Create(label, _host);
        var createdLabel = createLabelRequest.Execute();

        if (createdLabel is not null)
        {
            logger.LogInformation(
                "Label created: {labelName}, Label ID: {labelId}, BackgroundColor: {backgroundColor}, TextColor: {textColor}",
                createdLabel.Name, createdLabel.Id, backgroundColor, textColor);
            return true;
        }

        logger.LogError("Failed to create label {labelName}.", labelName);
        return false;
    }

    private string? GetLabelId(string? labelName)
    {
        try
        {
            if (labelName is null)
                return null;

            var labelsRequest = _gmailService!.Users.Labels.List(_host);
            var labelsResponse = labelsRequest?.Execute();

            var label = labelsResponse?.Labels?.FirstOrDefault(l => l.Name == labelName);

            return label?.Id;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetLabelName(LabelType labelType)
    {
        return labelType switch
        {
            LabelType.High => "MerrMail: High Priority",
            LabelType.Low => "MerrMail: Low Priority",
            LabelType.Other => "MerrMail: Other",
            LabelType.None => null,
            _ => throw new ArgumentOutOfRangeException(nameof(labelType), labelType, null)
        };
    }

    private bool MessageAnalyzed(Message message)
    {
        return message.LabelIds?.Any(label =>
            label == GetLabelId("MerrMail: High Priority") ||
            label == GetLabelId("MerrMail: Low Priority") ||
            label == GetLabelId("MerrMail: Other")) is true;
    }
}