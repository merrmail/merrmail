using MerrMail.Application.Contracts;
using MerrMail.Domain.Models;
using MerrMail.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MerrMail.Infrastructure.Services;

public partial class TensorFlowEmailAnalyzerService(
    ILogger<TensorFlowEmailAnalyzerService> logger,
    IOptions<EmailAnalyzerOptions> emailAnalyzerOptions) : IEmailAnalyzerService
{
    private readonly float _acceptedScore = emailAnalyzerOptions.Value.AcceptanceScore;

    /// <summary>
    /// Initializes the TensorFlow email analyzer service.
    /// </summary>
    /// <returns>True if initialization was successful; otherwise, false.</returns>
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

    /// <summary>
    /// Retrieves the best matching email reply based on email context and similarity scores.
    /// </summary>
    /// <param name="emailThread">The email thread to analyze.</param>
    /// <param name="emailContexts">The collection of email contexts to compare with.</param>
    /// <returns>The best matching email reply, or null if no suitable reply is found.</returns>
    public string? GetEmailReply(EmailThread emailThread, IEnumerable<EmailContext> emailContexts)
    {
        string? reply = null;
        var highestCosine = 0f;

        foreach (var emailContext in emailContexts)
        {
            logger.LogInformation("Getting similarity score of subject: {first} | {second}.", emailThread.Subject,
                emailContext.Subject);
            var subjectCosine = get_similarity_score(emailThread.Subject, emailContext.Subject);
            logger.LogInformation("Received a similarity score of {subjectCosine}.", subjectCosine);
            
            logger.LogInformation("Getting similarity score of body: {first} | {second}.", emailThread.Body,
                emailContext.Subject);
            var bodyCosine = get_similarity_score(emailThread.Body, emailContext.Subject);
            logger.LogInformation("Received a similarity score of {bodyCosine}.", bodyCosine);

            var cosine = subjectCosine >= bodyCosine ? subjectCosine : bodyCosine;

            if (cosine < highestCosine) continue;

            highestCosine = cosine;
            reply = emailContext.Response;
        }

        if (highestCosine > _acceptedScore) return reply;

        logger.LogInformation("No similar email found.");
        return null;
    }
}