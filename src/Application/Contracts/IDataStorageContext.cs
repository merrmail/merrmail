using MerrMail.Domain.Models;

namespace MerrMail.Application.Contracts;

/// <summary>
/// Represents a contract for retrieving email contexts from a data storage, which could be a database or a CSV file.
/// </summary>
public interface IDataStorageContext
{
    /// <summary>
    /// Retrieves all email contexts from the assigned data storage, either via a database or a CSV file.
    /// </summary>
    /// <returns>A list of email contexts.</returns>
    Task<IEnumerable<EmailContext>> GetEmailContextsAsync();
}