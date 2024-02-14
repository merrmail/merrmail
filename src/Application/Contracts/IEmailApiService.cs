using Merrsoft.MerrMail.Domain.Contracts;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    List<Email> GetUnreadEmails();
    Task Reply(string to);
    void MarkAsRead(string messageId);
}