namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IApplicationService
{
    Task<bool> CanStartAsync();
    Task RunAsync();
    Task StopAsync();
}