using Merrsoft.MerrMail.Application.Interfaces;

namespace Merrsoft.MerrMail.Presentation;

public class MerrMailWorker : BackgroundService
{
    private readonly ILogger<MerrMailWorker> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMerrMailService _merrMailService;

    public MerrMailWorker(ILogger<MerrMailWorker> logger, HttpClient httpClient, IMerrMailService merrMailService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _merrMailService = merrMailService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Merr Mail Background Service...");
        await _merrMailService.StartAsync();

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Merr Mail Background Service...");
        _httpClient.Dispose();
        await _merrMailService.StopAsync();

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
                _logger.LogWarning("Connection timeout. Retrying in 1s");
                
                continue;
            }
            
            _logger.LogInformation("No new emails found. Waiting for new emails");

            await _merrMailService.RunAsync();
        }
    }

    private async Task<bool> CheckInternetConnectionAsync()
    {
        try
        {
            using var response = await _httpClient.GetAsync("https://www.google.com");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}