namespace Merrsoft.MerrMail.Domain.Options;

public class TensorFlowBindingOptions
{
    public required string PythonDllFilePath { get; set; }
    public required string UniversalSentenceEncoderDirectoryPath { get; set; }
}