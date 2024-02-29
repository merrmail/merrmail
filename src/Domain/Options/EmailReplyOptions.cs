using System.ComponentModel.DataAnnotations;

namespace Merrsoft.MerrMail.Domain.Options;

public class EmailReplyOptions
{
    [Required] public required string Header { get; set; }
    [Required] public required string Introduction { get; set; }
    [Required] public required string Conclusion { get; set; }
    [Required] public required string Closing { get; set; }
    [Required] public required string Signature { get; set; }
}