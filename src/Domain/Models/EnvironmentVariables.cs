namespace Merrsoft.MerrMail.Domain.Models;

public record EnvironmentVariables(
    string OAuthClientCredentialsPath,
    string AccessTokenPath,
    string DatabaseConnection,
    string HostAddress);