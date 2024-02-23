using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Python.Runtime;

namespace Merrsoft.MerrMail.Infrastructure.Services;

public class AiIntegrationService(IOptions<TensorFlowBindingOptions> tensorFlowBindingOptions, ILogger<AiIntegrationService> logger) : IAiIntegrationService
{
    private dynamic? _scope;
    private readonly string _dll = tensorFlowBindingOptions.Value.PythonDllFilePath;

    // Since _scope is dynamic, this unused variable is used to check what you can do with the _scope
#pragma warning disable CS0169 // Field is never used
    private PyModule? _testScope;
#pragma warning restore CS0169 // Field is never used

    public bool Initialize()
    {
        try
        {
            logger.LogInformation("Setting up Python...");
            Runtime.PythonDLL = _dll;
            PythonEngine.Initialize();
            _scope = Py.CreateScope();
            logger.LogInformation("Python initialized");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to start AI Integration: {message}", ex.Message);
            return false;
        }

        return true;
    }
}