using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace InventoryStudio.FileHandlers
{
    public class CsvFileHandler : IFileHandler
    {
        public async Task<List<string[]>> ImportTemplate(IFormFile file)
        {
            var list = new List<string[]>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.Default))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }))
                {
                    string[] headers = null;
                    while (await csv.ReadAsync())
                    {
                        var currentLine = csv.Parser.RawRecord;
                        await Console.Out.WriteLineAsync(currentLine);
                        if (currentLine.StartsWith("---"))
                            continue;
                        headers = csv.Parser.Record;
                        if (headers != null)
                        {
                            list.Add(headers);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<byte[]> ExportTemplate(string[] headerFields)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true)))
                {
                    var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                    csvWriter.WriteField(headerFields);
                    await csvWriter.NextRecordAsync();
                    streamWriter.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task<byte[]> ExportImportResult(List<string> datas)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (datas.Any())
                {
                    dynamic jsonData = JsonConvert.DeserializeObject<ExpandoObject>(datas[0], new ExpandoObjectConverter());
                    var properties = ((IDictionary<string, object>)jsonData).Keys;

                    foreach (var property in properties)
                    {
                        csv.WriteField(property);
                    }
                    await csv.NextRecordAsync();
                }

                foreach (var data in datas)
                {
                    dynamic jsonData = JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());
                    csv.WriteRecord(jsonData);
                    await csv.NextRecordAsync();
                }

                writer.Flush();
                return memoryStream.ToArray();
            }
        }

        public async Task<List<Dictionary<string, string>>> ImportData(IFormFile file)
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

        public async Task<List<List<Dictionary<string, string>>>> ImportDatas(IFormFile file)
        {
            var result = new List<List<Dictionary<string, string>>>();
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }))
            {
                string[] headers = null;
                var currentDataList = new List<Dictionary<string, string>>();
                while (await csv.ReadAsync())
                {
                    var currentLine = csv.Parser.RawRecord;
                    if (currentLine.StartsWith("---"))
                    {
                        if (headers != null)
                        {
                            result.Add(currentDataList);
                            currentDataList = new List<Dictionary<string, string>>();
                        }
                        headers = null;
                        continue;
                    }
                    if (headers == null)
                        headers = csv.Parser.Record;
                    if (headers != null)
                    {
                        var rowData = new Dictionary<string, string>();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var header = headers[i];
                            var value = csv.Parser.Record[i];
                            if (header.Equals(value))
                                continue;
                            rowData[headers[i]] = csv.Parser.Record[i];
                        }
                        if (rowData.Count > 0)
                            currentDataList.Add(rowData);
                    }
                }
                if (headers != null)
                    result.Add(currentDataList);
            }
            return result;
        }
    }

}
