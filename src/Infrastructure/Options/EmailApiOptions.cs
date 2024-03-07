namespace Merrsoft.MerrMail.Infrastructure.Options;

public class EmailApiOptions
{
    public required string OAuthClientCredentialsFilePath { get; set; }
    public required string AccessTokenDirectoryPath { get; set; }
    public required string HostAddress { get; set; }
    public required string HostPassword { get; set; }
}