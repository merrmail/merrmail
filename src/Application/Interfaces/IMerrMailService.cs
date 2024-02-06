namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IMerrMailService
{
    Task<bool> CanStartAsync();
    Task RunAsync();
    Task StopAsync();
}