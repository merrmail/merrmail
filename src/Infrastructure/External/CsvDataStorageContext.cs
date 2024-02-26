using Merrsoft.MerrMail.Application.Contracts;
using Merrsoft.MerrMail.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

namespace Merrsoft.MerrMail.Infrastructure.External;

public class CsvDataStorageContext(ILogger<CsvDataStorageContext> logger) : IDataStorageContext
{
    public async Task<IEnumerable<EmailContext>> GetEmailContextsAsync(string access)
    {
        try
        {
            var emailContexts = new List<EmailContext>();
            string csvData;

            using (var reader = new StreamReader(access))
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