using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    Task<bool> InitializeAsync();
    Task Reply(string to);
    void MarkAsRead(string messageId);
    List<EmailThread> GetThreadsMessage();
}