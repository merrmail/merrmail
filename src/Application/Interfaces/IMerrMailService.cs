namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IMerrMailService
{
    Task StartAsync();
    Task RunAsync();
    Task StopAsync();
}