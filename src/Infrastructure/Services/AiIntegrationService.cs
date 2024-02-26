using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Python.Runtime;

// ReSharper disable InconsistentNaming

namespace Merrsoft.MerrMail.Infrastructure.Services;

public class AiIntegrationService(
    IOptions<AiIntegrationOptions> aiIntegrationOptions,
    ILogger<AiIntegrationService> logger) : IAiIntegrationService
{
    private dynamic? _scope;
    private dynamic? _embed;
    private readonly string _dll = aiIntegrationOptions.Value.PythonDllFilePath;
    private readonly float _acceptedScore = aiIntegrationOptions.Value.AcceptanceScore;

    // Since _scope is dynamic, this unused variable can be used to check what you can do with the _scope
#pragma warning disable CS0169 // Field is never used
    private readonly PyModule? _test;
#pragma warning restore CS0169 // Field is never used

    private readonly string universal_sentence_encoder_path =
        aiIntegrationOptions.Value.UniversalSentenceEncoderDirectoryPath;

    public bool Initialize()
    {
        try
        {
            logger.LogInformation("Setting up Python...");

            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", _dll);
            PythonEngine.Initialize();
            _scope = Py.CreateScope();

            logger.LogInformation("Python initialized.");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to start Python: {message}", ex.Message);
            return false;
        }

        try
        {
            logger.LogInformation("Setting up TensorFlow...");
            var directory = Directory.GetCurrentDirectory();
            var relativePath = Path.Combine("..", "Infrastructure", "Scripts");
            var fullPath = Path.GetFullPath(Path.Combine(directory, relativePath));

            var init = File.ReadAllText(Path.Combine(fullPath, "__init__.py"));
            var tf = File.ReadAllText(Path.Combine(fullPath, "tensorflow.py"));

            _scope!.Exec(init);
            _scope!.Exec(tf);

            logger.LogInformation("Loading Universal Sentence Encoder...");
            _embed = _scope!.load_universal_sentence_encoder(universal_sentence_encoder_path);
            logger.LogInformation("Finished setting up TensorFlow.");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to import required Python libraries: {message}", ex.Message);
            return false;
        }
        finally
        {
            if (PythonEngine.IsInitialized)
                PythonEngine.BeginAllowThreads();
        }

        return true;
    }

    // There is a problem in the python script where the cosine similarity score is reversed.
    public bool IsSimilar(string first, string second)
    {
        var cosine_similarity = _scope!.calculate_cosine_similarity(_embed, first, second);
        var cosine = cosine_similarity.As<float>();

        return cosine < _acceptedScore;
    }
}