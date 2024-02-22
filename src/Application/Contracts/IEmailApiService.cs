using System.Reflection.Emit;
using Merrsoft.MerrMail.Domain.Enums;
using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailApiService
{
    Task<bool> InitializeAsync();
    EmailThread? GetEmailThread();
    void ReplyThread(EmailThread emailThread, string message);
    void MoveThread(string threadId, LabelType addLabel);
}