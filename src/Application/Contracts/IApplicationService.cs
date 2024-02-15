namespace Merrsoft.MerrMail.Application.Contracts;

public interface IApplicationService
{
    Task<bool> CanStartAsync();
    Task RunAsync();
    Task StopAsync();
}