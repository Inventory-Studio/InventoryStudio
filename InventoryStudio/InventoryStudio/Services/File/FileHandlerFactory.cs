using System.Reflection;

namespace InventoryStudio.File
{
    public class FileHandlerFactory
    {
        public static IFileHandler CreateFileHandler(string fileType)
        {
            if (fileType == ".xls" || fileType == ".xlsx")
            {
                return new ExcelFileHandler();
            }
            else if (fileType == ".csv")
            {
                return new CsvFileHandler();
            }
            else
            {
                throw new NotSupportedException($"Unsupported file format: {fileType}");
            }
        }

        public static dynamic CreateFileHandlerInstance(string typeName, string fileType, string assemblyName = "ISLibrary")
        {
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    throw new ArgumentException($"Assembly not found: {assemblyName}");
                Type type = assembly.GetType(typeName);
                if (type == null)
                    throw new ArgumentException($"Type not found: {typeName}");
                Type factoryType = typeof(FileHandlerFactory);
                MethodInfo createFileHandlerMethod = factoryType.GetMethod(nameof(CreateFileHandler));
                MethodInfo genericMethod = createFileHandlerMethod.MakeGenericMethod(type);
                var fileHandlerInstance = genericMethod.Invoke(null, new object[] { fileType });
                return fileHandlerInstance;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating file handler instance: {ex.Message}");
            }
        }
    }

}
