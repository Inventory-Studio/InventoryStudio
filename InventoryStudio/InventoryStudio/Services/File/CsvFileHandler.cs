using CsvHelper;
using System.Globalization;

namespace InventoryStudio.File
{
    public class CsvFileHandler<T> : IFileHandler<T>
    {
        public async Task<byte[]> DownloadFileAsync(IEnumerable<T> records)
        {
            using (var writer = new StringWriter())
            {
                var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csvWriter.WriteHeader<T>();
                await csvWriter.WriteRecordsAsync(records);
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

        public async Task<List<T>> UploadFileAsync(IFormFile file)
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
    }

}
