using System.Reflection.Emit;
using Merrsoft.MerrMail.Domain.Enums;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    Task<bool> InitializeAsync();
    Task Reply(string to);
    void LabelThread(LabelType labelType, EmailThread emailThread);
    List<EmailThread> GetThreadsMessage();
    EmailThread? GetEmailThread();
    void ReplyThread(EmailThread emailThread, string message);
    void MoveThread(string threadId, LabelType addLabel);
    bool ThreadShouldAnalyze(EmailThread emailThread);
    void ArchiveThread(EmailThread emailThread);
}