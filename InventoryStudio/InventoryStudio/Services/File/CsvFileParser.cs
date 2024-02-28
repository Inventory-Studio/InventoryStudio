using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace InventoryStudio.Services.File
{
    public class CsvFileParser : IFileParser
    {
        public async Task<List<Dictionary<string, string>>> Parse(Stream fileStream)
        {
            var result = new List<Dictionary<string, string>>();

            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                await csv.ReadAsync();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;

                while (await csv.ReadAsync())
                {
                    var rowData = new Dictionary<string, string>();
                    foreach (var header in headers)
                    {
                        rowData[header] = csv.GetField(header);
                    }
                    result.Add(rowData);
                }
            }

            return result;
        }
    }
}
