namespace Merrsoft.MerrMail.Infrastructure.Options;

public class EmailReplyOptions
{
    public required string Header { get; set; }
    public required string Introduction { get; set; }
    public required string Conclusion { get; set; }
    public required string Closing { get; set; }
    public required string Signature { get; set; }
}