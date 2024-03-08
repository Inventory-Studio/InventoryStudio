using System.Reflection;

namespace InventoryStudio.FileHandlers
{
    public class FileHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FileHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileHandler CreateFileHandler(string fileType)
        {
            IFileHandler fileHandler;
            if (fileType == ".xls" || fileType == ".xlsx")
            {
                fileHandler = _serviceProvider.GetRequiredService<ExcelFileHandler>();
            }
            else if (fileType == ".csv")
            {
                fileHandler = _serviceProvider.GetRequiredService<CsvFileHandler>();
            }
            else
            {
                throw new NotSupportedException($"Unsupported file format: {fileType}");
            }
            return fileHandler;
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
