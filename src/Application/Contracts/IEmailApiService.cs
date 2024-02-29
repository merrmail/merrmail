using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Domain.Types;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    Task<bool> InitializeAsync();
    /// <summary>
    /// Retrieves a single EmailThread while filtering out unnecessary threads to optimize API usage.
    /// </summary>
    /// <returns>An EmailThread that has passed all validation checks, or null if no suitable thread is found.</returns>
    EmailThread? GetEmailThread();
    void MoveThread(string threadId, LabelType addLabel);
}