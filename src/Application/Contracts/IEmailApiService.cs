using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    Task<bool> InitializeAsync();
    List<Email> GetUnreadEmails();
    Task Reply(string to);
    void MarkAsRead(string messageId);
}