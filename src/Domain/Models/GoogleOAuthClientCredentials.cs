using Newtonsoft.Json;

namespace Merrsoft.MerrMail.Domain.Models;

public class GoogleOAuthClientCredentials
{
    [JsonProperty("installed")] public required Installed Installed { get; set; }
}

public class Installed
{
    [JsonProperty("client_id")] public required string ClientId { get; init; }

    [JsonProperty("project_id")] public required string ProjectId { get; init; }

    [JsonProperty("auth_uri")] public required string AuthUri { get; init; }

    [JsonProperty("token_uri")] public required string TokenUri { get; init; }

    [JsonProperty("auth_provider_x509_cert_url")]
    public required string AuthProviderX509CertUrl { get; init; }

    [JsonProperty("client_secret")] public required string ClientSecret { get; init; }

    [JsonProperty("redirect_uris")] public required string[] RedirectUris { get; init; }
}