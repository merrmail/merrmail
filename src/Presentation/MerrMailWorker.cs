using Merrsoft.MerrMail.Application.Contracts;

namespace Merrsoft.MerrMail.Presentation;

public class MerrMailWorker(
    ILogger<MerrMailWorker> logger,
    HttpClient httpClient,
    IApplicationService applicationService)
    : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Merr Mail Background Service...");

        if (!await applicationService.CanStartAsync())
        {
            logger.LogError("Unable to start Merr Mail Service!");

            await StopAsync(cancellationToken);
            return;
        }

        logger.LogInformation("Configuration is valid! Started reading emails...");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Merr Mail Background Service...");
        httpClient.Dispose();
        // await _applicationService.StopAsync();

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

            var hasInternet = await CheckInternetConnectionAsync();

            if (!hasInternet)
            {
                logger.LogWarning("Connection timeout. Retrying in 1s...");

                continue;
            }

            await applicationService.RunAsync();

            // TODO: Delete this when _applicationService.StopAsync() is implemented
            // await StopAsync(stoppingToken);
        }
    }

    private async Task<bool> CheckInternetConnectionAsync()
    {
        try
        {
            using var response = await httpClient.GetAsync("https://www.google.com");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}