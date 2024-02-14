using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Application.Services;
using Merrsoft.MerrMail.Infrastructure.Readers;
using Merrsoft.MerrMail.Infrastructure.Services;
using Merrsoft.MerrMail.Presentation;
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

    Log.Information("Starting Merr Mail");
    Log.Information("Configuring Services");

    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<MerrMailWorker>();

            services.AddSingleton<HttpClient>();

            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IEmailApiService, GmailApiService>();
            services.AddSingleton<IConfigurationReader, EnvConfigurationReader>();
            services.AddSingleton<IOAuthClientCredentialsReader, GoogleOAuthClientCredentialsReader>();
        })
        .UseSerilog();

    var host = builder.Build();

    host.Run();
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