namespace MerrMail.Infrastructure.Options;

/// <summary>
/// Options for configuring email reply content.
/// </summary>
public class EmailReplyOptions
{
    /// <summary>
    /// Gets or sets the header of the email reply.
    /// </summary>
    public required string Header { get; init; }

    /// <summary>
    /// The introduction of the email reply.
    /// </summary>
    public required string Introduction { get; init; }

    /// <summary>
    /// The conclusion of the email reply.
    /// </summary>
    public required string Conclusion { get; init; }

    /// <summary>
    /// The closing of the email reply.
    /// </summary>
    public required string Closing { get; init; }

    /// <summary>
    /// The signature of the email reply.
    /// </summary>
    public required string Signature { get; init; }
}