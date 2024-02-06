using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IOAuthClientCredentialsReader
{
    Task<GoogleOAuthClientCredentials?> ReadCredentialsAsync(string id);
}