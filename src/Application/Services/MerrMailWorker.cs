using Merrsoft.MerrMail.Application.Contracts;
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
        logger.LogInformation("Starting Merr Mail Background Service...");

        if (!await HasInternetAsync())
        {
            logger.LogError("Connection Timeout! Unable to start Merr Mail Service!");
            return;
        }

        logger.LogInformation("Started reading emails...");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var emails = emailApiService.GetUnreadEmails();

            foreach (var email in emails)
            {
                // emailApiService.MarkAsRead(email.MessageId);
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

    private async Task<bool> HasInternetAsync()
    {
        try
        {
            using var response = await httpClient.GetAsync("https://mail.google.com");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}