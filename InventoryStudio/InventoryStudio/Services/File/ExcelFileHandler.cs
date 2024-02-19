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
                var records = new List<T>();
                for (int i = 0; i < _entityTypes.Count; i++)
                {
                    var entityType = _entityTypes[i];
                    var sheet = workbook.GetSheet(entityType.Name);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(i);
                    }
                    var properties = entityType.GetProperties();
                    var recordList = new List<T>();
                    for (int j = 1; j <= sheet.LastRowNum; j++)
                    {
                        var row = sheet.GetRow(j);
                        var record = (T)Activator.CreateInstance(entityType);

                        for (int k = 0; k < properties.Length; k++)
                        {
                            var cell = row.GetCell(k);
                            if (cell != null)
                            {
                                var value = Convert.ChangeType(cell.ToString(), properties[k].PropertyType);
                                properties[k].SetValue(record, value);
                            }
                        }
                        recordList.Add(record);
                    }
                    records.AddRange(recordList);
                }

                return records;
            }
        }
    }

}
