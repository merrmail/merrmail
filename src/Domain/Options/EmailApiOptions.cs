namespace Merrsoft.MerrMail.Domain.Options;

public class EmailApiOptions
{
    public required string OAuthClientCredentialsFilePath { get; set; }
    public required string AccessTokenDirectoryPath { get; set; }
    public required string HostAddress { get; set; }
}