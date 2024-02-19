namespace InventoryStudio.File
{
    public class FileHandlerFactory
    {
        public static IFileHandler<T> CreateFileHandler<T>(string fileType) where T : class
        {
            if (fileType == ".xls" || fileType == ".xlsx")
            {
                return new ExcelFileHandler<T>(typeof(T));
            }
            else if (fileType == ".csv")
            {
                return new CsvFileHandler<T>();
            }
            else
            {
                throw new NotSupportedException($"Unsupported file format: {fileType}");
            }
        }
    }

}
