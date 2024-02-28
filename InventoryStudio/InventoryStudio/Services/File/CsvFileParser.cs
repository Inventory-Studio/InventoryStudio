using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace InventoryStudio.Services.File
{
    public class CsvFileParser : IFileParser
    {
        public async Task<List<Dictionary<string, string>>> Parse(IFormFile file)
        {
            var result = new List<Dictionary<string, string>>();

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)))
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
