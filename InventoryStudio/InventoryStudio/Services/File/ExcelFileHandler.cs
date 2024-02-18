using NPOI.HSSF.Record;
using NPOI.XSSF.UserModel;

namespace InventoryStudio.File
{
    public class ExcelFileHandler<T> : IFileHandler<T> where T : class
    {
        private readonly List<Type> _entityTypes;

        public ExcelFileHandler(params Type[] entityTypes)
        {
            _entityTypes = entityTypes.ToList();
        }
        public async Task<byte[]> Export(params IEnumerable<T>[] recordLists)
        {
            using (var fileStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();

                for (int i = 0; i < _entityTypes.Count; i++)
                {
                    var entityType = _entityTypes[i];
                    var sheet = workbook.CreateSheet(entityType.Name);
                    var headerRow = sheet.CreateRow(0);
                    var properties = entityType.GetProperties();

                    for (int j = 0; j < properties.Length; j++)
                    {
                        headerRow.CreateCell(j).SetCellValue(properties[j].Name);
                    }

                    var recordList = recordLists[i].Cast<object>();
                    int rowIndex = 1;

                    foreach (var record in recordList)
                    {
                        var row = sheet.CreateRow(rowIndex);

                        for (int j = 0; j < properties.Length; j++)
                        {
                            var cell = row.CreateCell(j);
                            var value = properties[j].GetValue(record);

                            if (value != null)
                            {
                                cell.SetCellValue(value.ToString());
                            }
                        }

                        rowIndex++;
                    }
                }

                workbook.Write(fileStream);
                return await Task.FromResult(fileStream.ToArray());
            }
        }

        public async Task<List<T>> Import(IFormFile file)
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
