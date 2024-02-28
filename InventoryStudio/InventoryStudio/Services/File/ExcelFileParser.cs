using NPOI.XSSF.UserModel;

namespace InventoryStudio.Services.File
{
    public class ExcelFileParser : IFileParser
    {
        public async Task<List<Dictionary<string, string>>> Parse(Stream fileStream)
        {
            var result = new List<Dictionary<string, string>>();
            var workbook = new XSSFWorkbook(fileStream);
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
            return result;
        }
    }
}
