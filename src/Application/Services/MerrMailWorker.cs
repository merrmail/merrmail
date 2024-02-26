using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Domain.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Application.Services;

public class MerrMailWorker(
    ILogger<MerrMailWorker> logger,
    IEmailApiService emailApiService,
    IAiIntegrationService aiIntegrationService,
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

            // TODO: Check if an email is a concern using ML.NET

            // TODO: Use a database to store these values
            var contexts = new List<EmailContext>
            {
                new("School uniform stock availability",
                    "We're still waiting for new uniforms to arrive. But right now, students are allowed to wear casual clothes."),
                new("Balance payment location",
                    "You can pay your balance at the cashier in the Main Building. You can also pay it online via email at cashier@university.domain."),
                new("Reason for slow email response time",
                    "We apologize for the inconvenience, we're working our way on making an email assistant for a faster email responses from our administrators.")
            };

            var labelType = LabelType.High;
            foreach (var context in contexts)
            {
                var score = aiIntegrationService.GetSimilarityScore(emailThread.Subject, context.Subject);

                // There is a problem in the python script where the cosine similarity score is reversed.
                // TODO: Make the accepted score configurable on startup
                const float acceptedScore = -0.35f;
                if (score < acceptedScore)
                {
                    // TODO: Get the actual reply to the database
                    labelType = LabelType.Low;

                    emailApiService.ReplyThread(emailThread, context.Response);
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