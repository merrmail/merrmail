using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailAnalyzerService
{
    bool Initialize();
    string? GetEmailReply(EmailThread emailThread, IEnumerable<EmailContext> emailContexts);
}