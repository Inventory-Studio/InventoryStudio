namespace InventoryStudio.Services.File
{
    public class FileParserFactory : IFileParserFactory
    {
        public IFileParser CreateParser(string fileType)
        {
            // 根据文件类型返回对应的解析器
            switch (fileType)
            {
                case "Excel":
                    return new ExcelFileParser();
                // 可以添加其他类型的解析器，如 CSV
                case "Csv":
                    return new CsvFileParser();
                default:
                    throw new NotSupportedException("File type not supported");
            }
        }
    }
}
