using Merrsoft.MerrMail.Domain.Enums;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    Task<bool> InitializeAsync();
    Task Reply(string to);
    void MarkAsRead(string messageId);
    void LabelThread(LabelType labelType, EmailThread emailThread);
    List<EmailThread> GetThreadsMessage();
    bool ThreadShouldAnalyze(EmailThread emailThread);
    void ArchiveThread(EmailThread emailThread);
}