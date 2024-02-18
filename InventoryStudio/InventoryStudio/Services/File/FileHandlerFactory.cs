namespace InventoryStudio.File
{
    public class FileHandlerFactory
    {
        public static IFileHandler<T> CreateFileHandler<T>(string fileType) where T : class
        {
            if (fileType == ".xlsx" || fileType == ".xls")
            {
                return new ExcelFileHandler<T>();
            }
            else if (fileType == ".csv")
            {
                return new CsvFileHandler<T>();
            }
            else
            {
                throw new NotSupportedException("Unsupported file type");
            }
        }
    }

}
