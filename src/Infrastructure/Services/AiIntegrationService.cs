using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Python.Runtime;

// ReSharper disable InconsistentNaming

namespace Merrsoft.MerrMail.Infrastructure.Services;

public class AiIntegrationService(
    IOptions<TensorFlowBindingOptions> tensorFlowBindingOptions,
    ILogger<AiIntegrationService> logger) : IAiIntegrationService
{
    private dynamic? _scope;
    private dynamic? _embed;
    private string? _fullPath;
    private readonly string _dll = tensorFlowBindingOptions.Value.PythonDllFilePath;

    private readonly string universal_sentence_encoder_path =
        tensorFlowBindingOptions.Value.UniversalSentenceEncoderDirectoryPath;

    // Since _scope is dynamic, this unused variable can be used to check what you can do with the _scope
#pragma warning disable CS0169 // Field is never used
    private PyModule? _testScope;
#pragma warning restore CS0169 // Field is never used

    public bool Initialize()
    {
        try
        {
            logger.LogInformation("Setting up Python...");

            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", _dll);
            PythonEngine.Initialize();
            _scope = Py.CreateScope();
            // PythonEngine.BeginAllowThreads();


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
            _fullPath = Path.GetFullPath(Path.Combine(directory, relativePath));

            var init = File.ReadAllText(Path.Combine(_fullPath, "__init__.py"));
            var tf = File.ReadAllText(Path.Combine(_fullPath, "tensorflow.py"));

            // PythonEngine.Initialize();
            // using (Py.GIL())
            // {
            _scope.Exec(init);
            _scope.Exec(tf);

            logger.LogInformation("Loading Universal Sentence Encoder...");
            _embed = _scope.load_universal_sentence_encoder(universal_sentence_encoder_path);
            // // var convertedPath = _universalSentenceEncoder.Replace(@"\", @"\\");
            // // _scope.Exec($"embed = load_universal_sentence_encoder('{convertedPath}')");
            // _scope.Exec($"embed = load_universal_sentence_encoder(r'{_universalSentenceEncoder}')");
            // _scope.Exec($"embed = 1");
            // _embed = _scope!.Eval("embed");
            // }

            // PythonEngine.BeginAllowThreads();
            logger.LogInformation("Finished setting up TensorFlow.");
            // PythonEngine.BeginAllowThreads();
        }
        catch (Exception ex)
        {
            // PythonEngine.BeginAllowThreads();
            logger.LogError("Failed to import required Python libraries: {message}", ex.Message);
            return false;
        }
        finally
        {
            PythonEngine.BeginAllowThreads();
            // Console.CancelKeyPress += (_, e) => e.Cancel = true;
        }

        return true;
    }

    public float GetSimilarityScore(string first, string second)
    {
        float cosine;
        // PythonEngine.Initialize();
        // using (Py.GIL())
        // {
        var cosine_similarity = _scope!.calculate_cosine_similarity(_embed, first, second);
        cosine = cosine_similarity.As<float>();
        // }
        // PythonEngine.BeginAllowThreads();

        return cosine;
    }
}