namespace Merrsoft.MerrMail.Domain.Options;

public class AiIntegrationOptions
{
    public required string PythonDllFilePath { get; set; }
    public required string UniversalSentenceEncoderDirectoryPath { get; set; }
    public required float AcceptanceScore { get; set; }
}