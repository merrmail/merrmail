namespace MerrMail.Infrastructure.Options;

/// <summary>
/// Options for configuring the email API.
/// </summary>
public class EmailApiOptions
{
    /// <summary>
    /// The file path to the OAuth 2.0 client credentials file.
    /// </summary>
    public required string OAuthClientCredentialsFilePath { get; init; }

    /// <summary>
    /// The directory path to store access tokens.
    /// </summary>
    public required string AccessTokenDirectoryPath { get; init; }

    /// <summary>
    /// The host address for the host.
    /// </summary>
    public required string HostAddress { get; init; }

    /// <summary>
    /// The host password for the host.
    /// </summary>
    public required string HostPassword { get; init; }
}