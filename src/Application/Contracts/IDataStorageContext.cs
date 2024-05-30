using Merrsoft.MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;

public interface IDataStorageContext
{
    Task<IEnumerable<EmailContext>> GetEmailContextsAsync();
}