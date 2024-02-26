using System.ComponentModel.DataAnnotations;
using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Application.Services;
using Merrsoft.MerrMail.Domain.Options;
using Merrsoft.MerrMail.Infrastructure.Factories;
using Merrsoft.MerrMail.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] - {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    Log.Information("Welcome to Merr Mail!");
    Log.Information("Configuring Services...");
    
    var builder = Host.CreateApplicationBuilder(args);

    #region Application Options

    builder.Services
        .AddOptions<EmailApiOptions>()
        .BindConfiguration($"{nameof(EmailApiOptions)}")
        .Validate(options => File.Exists(options.OAuthClientCredentialsFilePath),
            $"{nameof(EmailApiOptions.OAuthClientCredentialsFilePath)} does not exists")
        .Validate(options => Directory.Exists(options.AccessTokenDirectoryPath),
            $"{nameof(EmailApiOptions.AccessTokenDirectoryPath)} does not exists")
        .Validate(options =>
                new EmailAddressAttribute().IsValid(options.HostAddress),
            $"{nameof(EmailApiOptions.HostAddress)} is not a valid email")
        .Validate(options => !string.IsNullOrEmpty(options.HostPassword),
            $"Invalid {nameof(EmailApiOptions.HostPassword)}")
        .ValidateOnStart();

    builder.Services
        .AddOptions<DataStorageOptions>()
        .BindConfiguration($"{nameof(DataStorageOptions)}")
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services
        .AddOptions<TensorFlowBindingOptions>()
        .BindConfiguration($"{nameof(TensorFlowBindingOptions)}")
        .Validate(options => File.Exists(options.PythonDllFilePath),
            $"{nameof(TensorFlowBindingOptions.PythonDllFilePath)} does not exists")
        .Validate(options => Directory.Exists(options.UniversalSentenceEncoderDirectoryPath),
            $"{nameof(TensorFlowBindingOptions.UniversalSentenceEncoderDirectoryPath)} does not exists")
        .ValidateOnStart();

    #endregion
    
    #region Services
    
    builder.Services.AddSerilog();
    builder.Services.AddHttpClient();

    builder.Services.AddHostedService<MerrMailWorker>();

    builder.Services.AddSingleton<IEmailApiService, GmailApiService>();
    builder.Services.AddSingleton<IAiIntegrationService, AiIntegrationService>();

    builder.Services.AddSingleton<DataStorageContextFactory>(provider =>
    {
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var dataStorageOptions = provider.GetRequiredService<IOptions<DataStorageOptions>>();
        return new DataStorageContextFactory(loggerFactory, dataStorageOptions);
    });

    builder.Services.AddTransient<IDataStorageContext>(provider =>
    {
        var factory = provider.GetRequiredService<DataStorageContextFactory>();
        return factory.CreateDataStorageContext();
    });
    
    #endregion

    var host = builder.Build();

    Log.Information("Services Configured!");

    host.Run(); // Go to Application.Services.MerrMailWorker to see the background service that holds everything together
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.ToString());
}
finally
{
    Log.Information("Stopping Merr Mail");
    Log.CloseAndFlush();
}