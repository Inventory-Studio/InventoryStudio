using NPOI.HSSF.Record;
using NPOI.XSSF.UserModel;

namespace InventoryStudio.File
{
    public class ExcelFileHandler<T> : IFileHandler<T>
    {
        public Task<byte[]> DownloadFileAsync(IEnumerable<T> records)
        {
            using (var fileStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet("Sheet1");
                var headerRow = sheet.CreateRow(0);
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(properties[i].Name);
                }
                int rowIndex = 1;
                foreach (var record in records)
                {
                    var row = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var cell = row.CreateCell(i);
                        var value = properties[i].GetValue(record);
                        if (value != null)
                        {
                            cell.SetCellValue(value.ToString());
                        }
                    }
                    rowIndex++;
                }
                workbook.Write(fileStream);
                return Task.FromResult(fileStream.ToArray());
            }
        }

        public async Task<List<T>> UploadFileAsync(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var workbook = new XSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);
                var properties = typeof(T).GetProperties();
                var records = new List<T>();
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    var record = Activator.CreateInstance<T>();
                    for (int j = 0; j < properties.Length; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell != null)
                        {
                            var value = Convert.ChangeType(cell.ToString(), properties[j].PropertyType);
                            properties[j].SetValue(record, value);
                        }
                    }
                    records.Add(record);
                }
                return records;
            }
        }
    }

}
