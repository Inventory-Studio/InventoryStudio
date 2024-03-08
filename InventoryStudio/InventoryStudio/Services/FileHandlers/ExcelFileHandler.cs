using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Dynamic;

namespace InventoryStudio.FileHandlers
{
    public class ExcelFileHandler : IFileHandler
    {
        public async Task<List<string[]>> ImportTemplate(IFormFile file)
        {
            var list = new List<string[]>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var workbook = new XSSFWorkbook(stream);
                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    var sheet = workbook.GetSheetAt(sheetIndex);
                    var headerRow = sheet.GetRow(0);
                    var headerValues = new List<string>();
                    for (int i = 0; i < headerRow.LastCellNum; i++)
                    {
                        var cell = headerRow.GetCell(i);
                        headerValues.Add(cell.ToString());
                    }
                    list.Add(headerValues.ToArray());
                }
            }
            return list;
        }

        public async Task<byte[]> ExportTemplate(string[] headerFields)
        {
            using (var fileStream = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet();
                var headerRow = sheet.CreateRow(0);
                for (int i = 0; i < headerFields.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(headerFields[i]);
                }
                workbook.Write(fileStream);
                return await Task.FromResult(fileStream.ToArray());
            }
        }

        public async Task<byte[]> ExportImportResult(List<string> datas)
        {
            using (var memoryStream = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                if (datas.Any())
                {
                    dynamic jsonData = JsonConvert.DeserializeObject<ExpandoObject>(datas[0], new ExpandoObjectConverter());
                    var properties = ((IDictionary<string, object>)jsonData).Keys;

                    IRow headerRow = sheet.CreateRow(0);
                    int cellIndex = 0;
                    foreach (var property in properties)
                    {
                        headerRow.CreateCell(cellIndex).SetCellValue(property);
                        cellIndex++;
                    }
                }

                int rowIndex = 1;
                foreach (var data in datas)
                {
                    dynamic jsonData = JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    foreach (var value in jsonData)
                    {
                        dataRow.CreateCell(cellIndex).SetCellValue(value.Value.ToString());
                        cellIndex++;
                    }
                    rowIndex++;
                }

                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public Task<List<Dictionary<string, string>>> ImportData(IFormFile file)
        {
            var result = new List<Dictionary<string, string>>();
            using (var stream = file.OpenReadStream())
            {
                var workbook = new XSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                var headerRow = sheet.GetRow(0);
                var columnCount = headerRow.LastCellNum;
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var dataRow = sheet.GetRow(i);
                    if (dataRow != null)
                    {
                        var rowData = new Dictionary<string, string>();

                        for (int j = 0; j < columnCount; j++)
                        {
                            var cell = dataRow.GetCell(j);
                            if (cell != null)
                            {
                                rowData[headerRow.GetCell(j).StringCellValue] = cell.ToString();
                            }
                        }

                        result.Add(rowData);
                    }
                }
            }
            return Task.FromResult(result);
        }

        public Task<List<Dictionary<string, string>>> ImportDatas(IFormFile file)
        {
            var result = new List<Dictionary<string, string>>();
            using (var stream = file.OpenReadStream())
            {
                var workbook = new XSSFWorkbook(stream);
                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetIndex);
                    string[] headers = null;
                    for (int rowIndex = 0; rowIndex < sheet.PhysicalNumberOfRows; rowIndex++)
                    {
                        IRow row = sheet.GetRow(rowIndex);
                        if (row == null || row.Cells.All(cell => string.IsNullOrEmpty(cell.ToString()))) continue;
                        headers = row.Cells.Select(cell => cell.ToString()).ToArray();
                        if (headers != null)
                        {
                            for (int i = rowIndex + 1; i < sheet.PhysicalNumberOfRows; i++)
                            {
                                IRow dataRow = sheet.GetRow(i);
                                if (dataRow == null || dataRow.Cells.All(cell => string.IsNullOrEmpty(cell.ToString()))) continue;
                                var rowData = new Dictionary<string, string>();
                                for (int j = 0; j < headers.Length; j++)
                                {
                                    rowData[headers[j]] = dataRow.GetCell(j)?.ToString();
                                }
                                result.Add(rowData);
                            }
                        }
                    }
                }
            }
            return Task.FromResult(result);
        }
    }
}
