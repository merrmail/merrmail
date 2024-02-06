namespace Merrsoft.MerrMail.Domain.Models;

public class EnvironmentVariables
{
    public required string OAuthClientCredentialsPath { get; init; }
    public required string AccessTokenPath { get; init; }
    public required string DatabaseConnection { get; init; }
    public required string HostAddress { get; init; }
}