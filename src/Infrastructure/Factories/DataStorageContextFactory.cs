using MerrMail.Application.Contracts;
using MerrMail.Infrastructure.External;
using MerrMail.Infrastructure.Options;
using MerrMail.Infrastructure.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MerrMail.Infrastructure.Factories;

/// <summary>
/// A factory class for creating instances of IDataStorageContext based on the configured data storage type.
/// </summary>
/// <param name="loggerFactory">The factory used to create logger instances.</param>
/// <param name="dataStorageOptions">The options containing the data storage type chosen.</param>
public class DataStorageContextFactory(ILoggerFactory loggerFactory, IOptions<DataStorageOptions> dataStorageOptions)
{
    private readonly DataStorageType _dataStorageType = dataStorageOptions.Value.DataStorageType;

    /// <summary>
    /// Creates an instance of IDataStorageContext based on the configured data storage type.
    /// </summary>
    /// <returns>An instance of IDataStorageContext.</returns>
    /// <exception cref="NotSupportedException">Thrown when the data storage type is not supported.</exception>
    public IDataStorageContext CreateDataStorageContext()
    {
        switch (_dataStorageType)
        {
            case DataStorageType.Sqlite:
                var sqliteLogger = loggerFactory.CreateLogger<SqliteDataStorageContext>();
                return new SqliteDataStorageContext(sqliteLogger, dataStorageOptions);
            case DataStorageType.Csv:
                var csvLogger = loggerFactory.CreateLogger<CsvDataStorageContext>();
                return new CsvDataStorageContext(csvLogger, dataStorageOptions);
            default:
                throw new NotSupportedException($"Data storage type {_dataStorageType} is not supported.");
        }
    }
}