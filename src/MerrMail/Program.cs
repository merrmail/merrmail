using System.ComponentModel.DataAnnotations;
using MerrMail.Application.Contracts;
using MerrMail.Application.Services;
using MerrMail.Infrastructure.Factories;
using MerrMail.Infrastructure.Options;
using MerrMail.Infrastructure.Services;
using Merrsoft.MerrMail.Infrastructure.Types;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using TensorFlowEmailAnalyzerService = MerrMail.Infrastructure.Services.TensorFlowEmailAnalyzerService;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] - {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    Log.Information("Welcome to MerrMail!");
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
        .AddOptions<EmailReplyOptions>()
        .BindConfiguration($"{nameof(EmailReplyOptions)}")
        .Validate(options => !string.IsNullOrEmpty(options.Header),
            $"Invalid {nameof(EmailReplyOptions.Header)}")
        .Validate(options => !string.IsNullOrEmpty(options.Introduction),
            $"Invalid {nameof(EmailReplyOptions.Introduction)}")
        .Validate(options => !string.IsNullOrEmpty(options.Conclusion),
            $"Invalid {nameof(EmailReplyOptions.Conclusion)}")
        .Validate(options => !string.IsNullOrEmpty(options.Closing),
            $"Invalid {nameof(EmailReplyOptions.Closing)}")
        .Validate(options => !string.IsNullOrEmpty(options.Signature),
            $"Invalid {nameof(EmailReplyOptions.Signature)}")
        .ValidateOnStart();

    builder.Services
        .AddOptions<DataStorageOptions>()
        .BindConfiguration($"{nameof(DataStorageOptions)}")
        .Validate(options => !string.IsNullOrEmpty(options.DataStorageAccess),
            $"Invalid {nameof(DataStorageOptions.DataStorageAccess)}")
        .Validate(options => Enum.IsDefined(typeof(DataStorageType), options.DataStorageType),
            $"Invalid {nameof(DataStorageOptions.DataStorageType)}")
        .ValidateOnStart();

    builder.Services
        .AddOptions<EmailAnalyzerOptions>()
        .BindConfiguration($"{nameof(EmailAnalyzerOptions)}")
        // Our recommended acceptance score is between 0.24 and 0.35
        .Validate(options => options.AcceptanceScore >= 0.0 || options.AcceptanceScore <= 1.0,
            $"{nameof(EmailAnalyzerOptions.AcceptanceScore)} should be between 0.0 and 1.0")
        .ValidateOnStart();

    #endregion

    #region Services
    
    builder.Services.AddSerilog();
    builder.Services.AddHttpClient();

    builder.Services.AddHostedService<MerrMailWorker>();

    builder.Services.AddSingleton<IEmailApiService, GmailApiService>();
    builder.Services.AddSingleton<IEmailReplyService, SmtpReplyService>();
    builder.Services.AddSingleton<IEmailAnalyzerService, TensorFlowEmailAnalyzerService>();

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
