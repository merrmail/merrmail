namespace Merrsoft.MerrMail.Application.Contracts;

public interface IAiIntegrationService
{
    bool Initialize();
    bool IsSimilar(string first, string second);
}