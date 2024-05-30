using MerrMail.Application.Contracts;
using MerrMail.Domain.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MerrMail.Application.Services;

/// <summary>
/// The main MerrMail worker service class that holds everything together, including email validation, processing, and replying.
/// </summary>
/// <param name="logger">The logger instance to log information and errors.</param>
/// <param name="emailApiService">The email API service for interacting with the email platform.</param>
/// <param name="emailReplyService">The email reply service for sending replies to email threads.</param>
/// <param name="emailAnalyzerService">The email analyzer service for analyzing email threads.</param>
/// <param name="dataStorageContext">The data storage context for retrieving email contexts.</param>
/// <param name="httpClient">The HTTP client for checking internet connectivity.</param>
public class MerrMailWorker(
    ILogger<MerrMailWorker> logger,
    IEmailApiService emailApiService,
    IEmailReplyService emailReplyService,
    IEmailAnalyzerService emailAnalyzerService,
    IDataStorageContext dataStorageContext,
    HttpClient httpClient)
    : BackgroundService
{
    /// <summary>
    /// Starts the MerrMail worker service with the necessary initializations.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
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

        if (!emailAnalyzerService.Initialize())
        {
            logger.LogError("Email Analyzer Service initialization failed. Aborting startup.");
            return;
        }

        logger.LogInformation("Validation Complete!");
        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Executes the main loop of the MerrMail worker, processing email threads and generating replies.
    /// </summary>
    /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
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
            // We also prefer speed than RAM usage, so we're getting all rows instead of iterating each row
            var contexts = await dataStorageContext.GetEmailContextsAsync();
            var labelType = LabelType.High;

            var reply = emailAnalyzerService.GetEmailReply(emailThread, contexts);
            if (reply is not null)
            {
                emailReplyService.ReplyThread(emailThread, reply);
                labelType = LabelType.Low;
            }

            emailApiService.MoveThread(emailThread.Id, labelType);
            await Task.Delay(1000, stoppingToken);
            // break; /* <== Comment this when you want to test the loop */
        }
    }

    /// <summary>
    /// Stops the MerrMail worker service.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Merr Mail Background Service...");
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Checks internet connectivity by attempting to access a specified URL.
    /// </summary>
    /// <returns>True if the internet connection is valid, otherwise false.</returns>
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