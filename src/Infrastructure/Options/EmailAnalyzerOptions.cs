namespace MerrMail.Infrastructure.Options;

/// <summary>
/// Options for configuring email analyzer service.
/// </summary>
public class EmailAnalyzerOptions
{
    /// <summary>
    /// The acceptance score threshold for email similarity.
    /// </summary>
    public required float AcceptanceScore { get; init; }
}