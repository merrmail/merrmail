using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class MerrMailWorker(
    ILogger<MerrMailWorker> logger,
    IEmailApiService emailApiService,
    HttpClient httpClient)
    : BackgroundService
{
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

            // TODO: Check if an email is a concern using ML.NET
            // TODO: Compare email to database using NLP

            const LabelType labelType = LabelType.Low;
            var replyMessage = Guid.NewGuid().ToString();
            var replied = emailApiService.ReplyThread(emailThread, replyMessage);
            if (replied) emailApiService.MoveThread(emailThread.Id, labelType);
            else logger.LogWarning("Thread {threadId} won't be moved.", emailThread.Id);

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