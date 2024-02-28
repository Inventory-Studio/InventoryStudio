using NPOI.XSSF.UserModel;

namespace InventoryStudio.Services.File
{
    public class ExcelFileParser : IFileParser
    {
        public Task<List<Dictionary<string, string>>> Parse(IFormFile file)
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
    }
}
