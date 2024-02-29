using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailReplyService
{
    void ReplyThread(EmailThread emailThread, string message);
}