using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailAnalyzerService
{
    bool Initialize();
    string? GetEmailReply(string email, IEnumerable<EmailContext> emailContexts);
}