using System.Reflection;

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

        public static dynamic CreateFileHandlerInstance(string typeName, string fileType)
        {
            Type type = Type.GetType(typeName);
            Type factoryType = typeof(FileHandlerFactory);
            MethodInfo createFileHandlerMethod = factoryType.GetMethod(nameof(CreateFileHandler));
            MethodInfo genericMethod = createFileHandlerMethod.MakeGenericMethod(type);
            var fileHandlerInstance = genericMethod.Invoke(null, new object[] { fileType });
            return fileHandlerInstance;
        }
    }

}
