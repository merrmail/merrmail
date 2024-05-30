using MerrMail.Domain.Models;
using MerrMail.Domain.Types;

namespace MerrMail.Application.Contracts;

public interface IEmailApiService
{
    /// <summary>
    /// Initializes the email API depending on the email platform being used.
    /// </summary>
    /// <returns>True if initialization is successful, otherwise false.</returns>
    Task<bool> InitializeAsync();

    /// <summary>
    /// Retrieves a single email thread while filtering out unnecessary threads to optimize API usage.
    /// </summary>
    /// <returns>An email thread that has passed all validation checks, or null if no suitable thread is found.</returns>
    EmailThread? GetEmailThread();
    
    /// <summary>
    /// Moves an email thread to a specified label.
    /// </summary>
    /// <param name="threadId">The ID of the email thread to move.</param>
    /// <param name="labelType">The label type to which the thread will be moved.</param>
    void MoveThread(string threadId, LabelType labelType);
}