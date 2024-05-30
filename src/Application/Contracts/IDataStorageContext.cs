using MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;


public interface IDataStorageContext
{
    /// <summary>
    /// Retrieves all email contexts from the assigned data storage, either via a database or a CSV file.
    /// </summary>
    /// <returns>A list of email contexts.</returns>
    Task<IEnumerable<EmailContext>> GetEmailContextsAsync();
}