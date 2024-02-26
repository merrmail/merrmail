namespace Merrsoft.MerrMail.Application.Contracts;

public interface IAiIntegrationService
{
    bool Initialize();
    float GetSimilarityScore(string first, string second);
}