using CsvHelper;
using System.Globalization;
using System.Text;

namespace InventoryStudio.File
{
    public class CsvFileHandler<T> : IFileHandler<T> where T : class
    {
        public async Task<byte[]> Export(params IEnumerable<T>[] records)
        {
            using (var writer = new StringWriter())
            {
                var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csvWriter.WriteHeader<T>();
                await csvWriter.WriteRecordsAsync(records[0]);
                using (var memoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memoryStream))
                    {
                        streamWriter.Write(writer.ToString());
                        streamWriter.Flush();
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        public async Task<List<T>> Import(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return await csv.GetRecordsAsync<T>().ToListAsync();
                }
            }
        }

        public async Task<string[]> GetHeader(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.Default))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    if (await csv.ReadAsync())
                    {
                        csv.ReadHeader();
                        return csv.HeaderRecord;
                    }
                }
            }
            return null;
        }

        public async Task<Dictionary<string, string>> MapHeadersToEntityProperties(string[] headerFields)
        {
            var entityTypeProperties = typeof(T).GetProperties().Select(p => p.Name).ToList();
            var mapping = new Dictionary<string, string?>();

            foreach (var entityTypeProperty in entityTypeProperties)
            {
                if (headerFields.Contains(entityTypeProperty))
                {
                    mapping[entityTypeProperty] = entityTypeProperty;
                }
                else
                {
                    mapping[entityTypeProperty] = null;
                }
            }
            return await Task.FromResult(mapping);
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
    }

}
