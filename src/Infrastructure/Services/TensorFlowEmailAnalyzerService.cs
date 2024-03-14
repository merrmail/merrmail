using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Merrsoft.MerrMail.Infrastructure.Services;

public partial class TensorFlowEmailAnalyzerService(
    ILogger<TensorFlowEmailAnalyzerService> logger,
    IOptions<EmailAnalyzerOptions> aiIntegrationOptions) : IEmailAnalyzerService
{
    private readonly float _acceptedScore = aiIntegrationOptions.Value.AcceptanceScore;

    public bool Initialize()
    {
        try
        {
            const string merrsoft = "Merrsoft";
            var cosine = get_cosine_similarity(merrsoft, merrsoft);
            
            const double maxSimilarity = -1.0;
            const double tolerance = 1e-15;
            if (Math.Abs(cosine - maxSimilarity) > tolerance)
            {
                logger.LogError(
                    "Cosine similarity ({cosine}) is not equal to the expected value ({maxSimilarity} within the specified tolerance {tolerance}.",
                    cosine, maxSimilarity, tolerance);
                return false;
            }

            logger.LogInformation("Python Initialized!");

            return true;
        }
        catch (Exception ex)
        {
            logger.LogCritical("Cannot initialize Python AI: {message}", ex.Message);
            return false;
        }
    }

    public bool IsSimilar(string first, string second)
    {
        logger.LogInformation("Getting cosine similarity score of {first} | {second}", first, second);
        var cosine = get_cosine_similarity(first, second);

        logger.LogInformation("Received a cosine similarity score of {cosine}.", cosine);
        return cosine < _acceptedScore;
    }
}