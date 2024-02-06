using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Interfaces;

public interface IConfigurationReader
{
    EnvironmentVariables ReadConfiguration();
}