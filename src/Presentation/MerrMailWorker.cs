using Merrsoft.MerrMail.Application.Interfaces;

namespace Merrsoft.MerrMail.Presentation;

public class MerrMailWorker : BackgroundService
{
    private readonly ILogger<MerrMailWorker> _logger;
    private readonly HttpClient _httpClient;
    private readonly IApplicationService _applicationService;

    public MerrMailWorker(ILogger<MerrMailWorker> logger, HttpClient httpClient, IApplicationService applicationService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _applicationService = applicationService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Merr Mail Background Service...");

        if (!await _applicationService.CanStartAsync())
        {
            _logger.LogError("Unable to start Merr Mail Service");

            await StopAsync(cancellationToken);
            return;
        }

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Merr Mail Background Service...");
        _httpClient.Dispose();
        await _applicationService.StopAsync();

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

            await _applicationService.RunAsync();
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