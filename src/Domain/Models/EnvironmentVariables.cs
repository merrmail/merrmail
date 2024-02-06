namespace Merrsoft.MerrMail.Domain.Models;

public class EnvironmentVariables
{
    public required string OAuthClientCredentialsPath { get; set; }
    public required string DatabaseConnection { get; set; }
}