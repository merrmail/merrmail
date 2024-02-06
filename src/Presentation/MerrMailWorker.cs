namespace Merrsoft.MerrMail.Presentation;

public class MerrMailWorker(ILogger<MerrMailWorker> logger, HttpClient httpClient) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Merr Mail Background Service...");
        
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Merr Mail Background Service...");
        httpClient.Dispose();
        
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
            
            var hasInternet = await CheckInternetConnectionAsync();

            if (!hasInternet)
            {
                logger.LogWarning("Connection timeout. Retrying in 1s");
                
                continue;
            }
            
            logger.LogInformation("No new emails found. Waiting for new emails");
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