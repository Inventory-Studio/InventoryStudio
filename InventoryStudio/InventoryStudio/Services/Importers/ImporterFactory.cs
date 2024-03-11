namespace InventoryStudio.Services.Importers
{
    public class ImporterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ImporterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IImporter CreateImporter(string type)
        {
            IImporter importer;
            switch (type)
            {
                case "Vendor":
                    importer = _serviceProvider.GetRequiredService<VendorImporter>();
                    break;
                case "Customer":
                    importer = _serviceProvider.GetRequiredService<CustomerImporter>();
                    break;
                case "Item":
                    importer = _serviceProvider.GetRequiredService<ItemImporter>();
                    break;
                case "SalesOrder":
                    importer = _serviceProvider.GetRequiredService<SalesOrderImporter>();
                    break;
                default:
                    throw new ArgumentException($"Invalid importer type: {type}");
            }
            return importer;
        }
    }
}
