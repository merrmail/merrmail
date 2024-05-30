using MerrMail.Application.Contracts;
using MerrMail.Infrastructure.External;
using MerrMail.Infrastructure.Options;
using Merrsoft.MerrMail.Infrastructure.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MerrMail.Infrastructure.Factories;

public class DataStorageContextFactory(ILoggerFactory loggerFactory, IOptions<DataStorageOptions> dataStorageOptions)
{
    private readonly DataStorageType _dataStorageType = dataStorageOptions.Value.DataStorageType;

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