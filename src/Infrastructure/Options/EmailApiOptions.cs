namespace Merrsoft.MerrMail.Infrastructure.Options;

public class EmailApiOptions
{
    public required string OAuthClientCredentialsFilePath { get; init; }
    public required string AccessTokenDirectoryPath { get; init; }
    public required string HostAddress { get; init; }
    public required string HostPassword { get; init; }
}