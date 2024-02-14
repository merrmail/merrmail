namespace Merrsoft.MerrMail.Domain.Models;

public class EnvironmentVariables
{
    public string OAuthClientCredentialsPath { get; set; } = string.Empty;
    public string AccessTokenPath { get; set; } = string.Empty;
    public string DatabaseConnection { get; set; } = string.Empty;
    public string HostAddress { get; set; } = string.Empty;
}