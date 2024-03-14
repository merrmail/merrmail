namespace Merrsoft.MerrMail.Application.Contracts;

public interface IEmailAnalyzerService
{
    bool Initialize();
    bool IsSimilar(string first, string second);
}