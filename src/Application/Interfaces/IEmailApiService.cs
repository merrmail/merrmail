using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IEmailApiService
{
    List<Email> GetUnreadEmails(EnvironmentVariables env);
    Task Reply(string to);
    void MarkMessageAsRead(EnvironmentVariables env, string messageId);
}