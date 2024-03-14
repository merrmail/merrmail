namespace Merrsoft.MerrMail.Infrastructure.Options;

public class EmailReplyOptions
{
    public required string Header { get; init; }
    public required string Introduction { get; init; }
    public required string Conclusion { get; init; }
    public required string Closing { get; init; }
    public required string Signature { get; init; }
}