using InventoryStudio.Models;
using InventoryStudio.Services.File;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using ISLibrary.OrderManagement;
using System.Security.Claims;

namespace InventoryStudio.Importer
{
    public class CustomerImporter
    {
        private readonly IFileParserFactory _fileParserFactory;


        public CustomerImporter(IFileParserFactory fileParserFactory)
        {
            _fileParserFactory = fileParserFactory;
        }

        public async Task ImportDataAsync(string companyId, string importTemplateID, string userId, IFormFile file, ProgressHandler progressHandler)
        {
            var fileType = Path.GetExtension(file.FileName);
            var parser = _fileParserFactory.CreateParser(fileType);
            var datas = await parser.Parse(file);
            var importTemplateFilter = new ImportTemplateFieldFilter();
            importTemplateFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            importTemplateFilter.ImportTemplateID.SearchString = importTemplateID;
            var importTemplateFields = ImportTemplateField.GetImportTemplateFields(companyId, importTemplateFilter);
            var fieldMappings = importTemplateFields
                .ToDictionary(f => f.SourceField, f => f.DestinationField);
            int totalDataCount = datas.Count;
            int processedCount = 0;
            var customers = new List<Customer>();
            foreach (var data in datas)
            {
                var customer = new Customer();
                foreach (var field in data)
                {
                    if (fieldMappings.ContainsKey(field.Key))
                    {
                        var destinationField = fieldMappings[field.Key];
                        var property = typeof(Customer).GetProperty(destinationField);
                        if (property != null)
                        {
                            if (destinationField == "Company")
                            {
                                CompanyFilter filter = new CompanyFilter();
                                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                                filter.CompanyName.SearchString = field.Value;
                                var companies = Company.GetCompanies(filter);
                                if (companies != null)
                                    property.SetValue(customer, companies.FirstOrDefault());
                            }
                            else if (destinationField == "Client")
                            {
                                ClientFilter filter = new ClientFilter();
                                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                                filter.EmailAddress.SearchString = field.Value;
                                var client = Client.GetClient(companyId, filter);
                                if (client != null)
                                    property.SetValue(customer, client);
                            }
                            else if (destinationField == "DefaultBillingAddress" || destinationField == "DefaultShippingAddress")
                            {
                                AddressFilter filter = new AddressFilter();
                                filter.FullName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                                filter.FullName.SearchString = field.Value;
                                var address = Address.GetAddress(companyId, filter);
                                if (address != null)
                                    property.SetValue(customer, address);
                            }
                            else
                            {
                                var value = Convert.ChangeType(field.Value, property.PropertyType);
                                property.SetValue(customer, value);
                            }
                        }
                    }
                }
                customer.CreatedBy = userId;
                customer.Create();
                processedCount++;
                int progress = (int)((processedCount / (double)totalDataCount) * 100);
                progressHandler?.Invoke(progress, importTemplateID);
            }
        }
    }
}
