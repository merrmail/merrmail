using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Options;
using Merrsoft.MerrMail.Domain.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Merrsoft.MerrMail.Application.Services;

public class MerrMailWorker(
    ILogger<MerrMailWorker> logger,
    IOptions<DataStorageOptions> dataStorageOptions,
    IEmailApiService emailApiService,
    IEmailReplyService emailReplyService,
    IAiIntegrationService aiIntegrationService,
    IDataStorageContext dataStorageContext,
    HttpClient httpClient)
    : BackgroundService
{
    private readonly string _dataStorageAccess = dataStorageOptions.Value.DataStorageAccess;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting validation...");

        if (!await CheckInternetAsync())
        {
            logger.LogError("Internet connection validation failed. Aborting startup.");
            return;
        }

        if (!await emailApiService.InitializeAsync())
        {
            logger.LogError("Email API service initialization failed. Aborting startup.");
            return;
        }

        if (!aiIntegrationService.Initialize())
        {
            logger.LogError("AI initialization failed. Aborting startup.");
            return;
        }

        logger.LogInformation("Validation Complete!");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        logger.LogInformation("Started reading emails...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var emailThread = emailApiService.GetEmailThread();
            if (emailThread is null)
            {
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            // We don't store email contexts once so the users of this program can still do CRUD operations on the database
            // We also prefer speed over RAM usage so we're getting all rows instead of iterating each row
            var contexts = await dataStorageContext.GetEmailContextsAsync(_dataStorageAccess);

            var labelType = LabelType.High;
            foreach (var context in contexts)
            {
                var similar = aiIntegrationService.IsSimilar(emailThread.Subject, context.Subject);
                if (similar)
                {
                    labelType = LabelType.Low;
                    emailReplyService.ReplyThread(emailThread, context.Response);
                }

                if (labelType is LabelType.Low) break;
            }

            emailApiService.MoveThread(emailThread.Id, labelType);
            await Task.Delay(1000, stoppingToken);
            // break; /* <== Comment this when you want to test the loop */
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Merr Mail Background Service...");
        await base.StopAsync(cancellationToken);
    }

    private async Task<bool> CheckInternetAsync()
    {
        try
        {
            using var response = await httpClient.GetAsync("https://mail.google.com");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            logger.LogError("Connection Timeout!");
            return false;
        }
    }
}