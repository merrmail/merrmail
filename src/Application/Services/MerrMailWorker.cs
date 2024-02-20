using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Enums;
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
            var threads = emailApiService.GetThreadsMessage();

            foreach (var thread in threads)
            {
                if (emailApiService.ThreadShouldAnalyze(thread))
                {
                    var random = new Random();
                    var labelType = random.Next(2) == 0 ? LabelType.High : LabelType.Low;
                    emailApiService.LabelThread(labelType, thread);
                }
                
                emailApiService.ArchiveThread(thread);
            }

            // TODO: Check if an email is a concern
            // TODO: Compare email to database
            // TODO: Label email 
            // TODO: Reply to email

            await Task.Delay(1000, stoppingToken);
            await StopAsync(stoppingToken); // <== Comment this when you want to test the loop
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