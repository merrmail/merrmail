using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IEmailApiService
{
    List<Email> GetUnreadEmails(EnvironmentVariables env);
    Task Reply(string to);
    void MarkAsRead(EnvironmentVariables env, string messageId);
}