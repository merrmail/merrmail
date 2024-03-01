using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Merrsoft.MerrMail.Infrastructure.External;

public class SqliteDataStorageContext(ILogger<SqliteDataStorageContext> logger) : IDataStorageContext
{
    public async Task<IEnumerable<EmailContext>> GetEmailContextsAsync(string access)
    {
        try
        {
            logger.LogInformation("Getting email contexts...");
            var connectionString = $"Data Source=file:{access}";
            var emailContexts = new List<EmailContext>();

            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqliteCommand($"SELECT Subject, Response FROM EmailContext", connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var emailContext = new EmailContext(
                    reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    reader.IsDBNull(1) ? string.Empty : reader.GetString(1));

                emailContexts.Add(emailContext);
            }

            return emailContexts;
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to get Email Contexts: {message}", ex.Message);
            return [];
        }
    }
}