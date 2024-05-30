using MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Merrsoft.MerrMail.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;

namespace Merrsoft.MerrMail.Infrastructure.External;

public class CsvDataStorageContext(ILogger<CsvDataStorageContext> logger, IOptions<DataStorageOptions> dataStorageOptions) : IDataStorageContext
{
    private readonly string _dataStorageAccess = dataStorageOptions.Value.DataStorageAccess;
    
    public async Task<IEnumerable<EmailContext>> GetEmailContextsAsync()
    {
        try
        {
            var emailContexts = new List<EmailContext>();
            string csvData;

            using (var reader = new StreamReader(_dataStorageAccess))
            {
                csvData = await reader.ReadToEndAsync();
            }

            using (var parser = new TextFieldParser(new StringReader(csvData)))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                parser.ReadFields();

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();

                    if (fields is not { Length: 2 }) continue;
                    var context = new EmailContext(fields[0], fields[1]);

                    emailContexts.Add(context);
                }
            }

            return emailContexts;
        }
        catch (Exception ex)
        {
            logger.LogError("Error parsing CSV: {message}", ex.Message);
            return [];
        }
    }
}