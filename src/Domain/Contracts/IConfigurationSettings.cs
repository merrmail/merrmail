namespace Merrsoft.MerrMail.Domain.Contracts;

public interface IConfigurationSettings
{
    string OAuthClientCredentialsPath { get; set; }
    string AccessTokenPath { get; set; }
    string DatabaseConnection { get; set; }
    string HostAddress { get; set; }
}