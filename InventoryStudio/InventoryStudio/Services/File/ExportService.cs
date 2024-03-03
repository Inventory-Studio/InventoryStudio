using CsvHelper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryStudio.Services.File
{
    public class ExportService
    {
        public async Task<byte[]> ExportJsonToCsv(List<string> datas)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (datas.Any())
                {
                    var jsonData = JsonSerializer.Deserialize<ExpandoObject>(datas[0]);
                    var properties = ((IDictionary<string, object>)jsonData).Keys;

                    foreach (var property in properties)
                    {
                        csv.WriteField(property);
                    }
                    await csv.NextRecordAsync();
                }

                foreach (var data in datas)
                {
                    var jsonData = JsonSerializer.Deserialize<ExpandoObject>(data);
                    csv.WriteRecord(jsonData);
                    await csv.NextRecordAsync();
                }

                writer.Flush();
                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportJsonToExcel(List<string> datas)
        {
            using (var memoryStream = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                if (datas.Any())
                {
                    var jsonData = JsonSerializer.Deserialize<ExpandoObject>(datas[0]);
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
                    var jsonData = JsonSerializer.Deserialize<ExpandoObject>(data);
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

        public string InsertErrorMessage(string json, string errorMessage)
        {
            var jsonData = JsonSerializer.Deserialize<ExpandoObject>(json);
            var newJsonData = new ExpandoObject() as IDictionary<string, object>;
            newJsonData["ErrorMessage"] = errorMessage;
            foreach (var property in jsonData)
            {
                newJsonData[property.Key] = property.Value;
            }
            return JsonSerializer.Serialize(newJsonData);
        }
    }
}
