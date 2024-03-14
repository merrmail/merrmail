using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Merrsoft.MerrMail.Infrastructure.Services;

public partial class TensorFlowEmailAnalyzerService(
    ILogger<TensorFlowEmailAnalyzerService> logger,
    IOptions<EmailAnalyzerOptions> emailAnalyzerOptions) : IEmailAnalyzerService
{
    private readonly float _acceptedScore = emailAnalyzerOptions.Value.AcceptanceScore;

    public bool Initialize()
    {
        try
        {
            const string merrsoft = "Merrsoft";
            var cosine = get_similarity_score(merrsoft, merrsoft);

            const double maxSimilarity = 1.0;
            const double tolerance = 1e-15;
            if (Math.Abs(cosine - maxSimilarity) > tolerance)
            {
                logger.LogError(
                    "Cosine similarity ({cosine}) is not equal to the expected value ({maxSimilarity} within the specified tolerance {tolerance}.",
                    cosine, maxSimilarity, tolerance);
                return false;
            }

            logger.LogInformation("TensorFlow Initialized!");

            return true;
        }
        catch (Exception ex)
        {
            logger.LogCritical("Cannot initialize TensorFlow: {message}", ex.Message);
            return false;
        }
    }

    public string? GetEmailReply(string subject, IEnumerable<EmailContext> emailContexts)
    {
        string? reply = null;
        var highestCosine = 0f;

        foreach (var emailContext in emailContexts)
        {
            logger.LogInformation("Getting similarity score of {first} | {second}.", subject,
                emailContext.Subject);
            var cosine = get_similarity_score(subject, emailContext.Subject);
            logger.LogInformation("Received a similarity score of {cosine}.", cosine);

            if (cosine < highestCosine) continue;

            highestCosine = cosine;
            reply = emailContext.Response;
        }

        if (highestCosine > _acceptedScore) return reply;

        logger.LogInformation("No similar email found.");
        return null;
    }
}