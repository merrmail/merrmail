using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IOAuthClientCredentialsReader
{
    Task<GoogleOAuthClientCredentials?> ReadCredentialsAsync(string id);
}