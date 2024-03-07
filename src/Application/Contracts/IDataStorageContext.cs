using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Application.Contracts;

public interface IDataStorageContext
{
    Task<IEnumerable<EmailContext>> GetEmailContextsAsync();
}