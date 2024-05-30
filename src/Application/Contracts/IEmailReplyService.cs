using MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;

public interface IEmailReplyService
{
    void ReplyThread(EmailThread emailThread, string message);
}