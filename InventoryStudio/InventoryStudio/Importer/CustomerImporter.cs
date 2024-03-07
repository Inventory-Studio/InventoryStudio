using InventoryStudio.File;
using InventoryStudio.Models;
using InventoryStudio.Services.File;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using ISLibrary.OrderManagement;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace InventoryStudio.Importer
{
    public class CustomerImporter
    {
        public async Task ImportDataAsync(string companyId, string importTemplateID, string userId, IFormFile file, ProgressHandler progressHandler)
        {
            var fileType = Path.GetExtension(file.FileName);
            var fileHandler = FileHandlerFactory.CreateFileHandler(fileType);
            var datas = await fileHandler.ImportData(file);
            var importTemplateFilter = new ImportTemplateFieldFilter();
            importTemplateFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            importTemplateFilter.ImportTemplateID.SearchString = importTemplateID;
            var importTemplateFields = ImportTemplateField.GetImportTemplateFields(companyId, importTemplateFilter);
            int processedCount = 0;
            var importResult = new ImportResult
            {
                ImportTemplateID = importTemplateID,
                UploadBy = userId,
                UploadTime = DateTime.Now,
                TotalRecords = datas.Count,
            };
            importResult.Create();
            foreach (var data in datas)
            {
                var customer = new Customer();
                foreach (var field in data)
                {
                    if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                    {
                        var property = typeof(Customer).GetProperty(destinationField);
                        if (property != null)
                        {
                            try
                            {
                                await SetPropertyAsync(companyId, customer, field.Value, property);
                            }
                            catch (Exception ex)
                            {
                                var failedDataJson = JsonSerializer.Serialize(data);
                                var failedRecord = new ImportFailedRecord
                                {
                                    ImportResultID = importResult.ImportResultID,
                                    ErrorMessage = ex.Message,
                                    FailedData = failedDataJson
                                };
                                failedRecord.Create();
                                importResult.FailedRecords++;
                            }
                        }
                    }
                }
                customer.CreatedBy = userId;
                customer.Create();
                processedCount++;
                importResult.SuccessfulRecords++;
                int progress = (int)((processedCount / (double)importResult.TotalRecords) * 100);
                progressHandler?.Invoke(progress, importTemplateID);
            }
            importResult.Update();
        }

        private bool TryGetDestinationField(string sourceField, List<ImportTemplateField> importTemplateFields, out string destinationField)
        {
            var field = importTemplateFields.FirstOrDefault(f => f.SourceField == sourceField);
            destinationField = field?.DestinationField;
            return field != null;
        }

        private async System.Threading.Tasks.Task SetPropertyAsync(string companyId, Customer customer, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Company))
            {
                CompanyFilter filter = new CompanyFilter();
                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CompanyName.SearchString = value;
                var companies = Company.GetCompanies(filter);
                if (companies != null)
                    property.SetValue(customer, companies.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(Client))
            {
                ClientFilter filter = new ClientFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var client = Client.GetClient(companyId, filter);
                if (client != null)
                    property.SetValue(customer, client);
            }
            else if (property.PropertyType == typeof(Address) && (property.Name == "DefaultBillingAddress" || property.Name == "DefaultShippingAddress"))
            {
                AddressFilter filter = new AddressFilter();
                filter.FullName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.FullName.SearchString = value;
                var address = Address.GetAddress(companyId, filter);
                if (address != null)
                    property.SetValue(customer, address);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(customer, convertedValue);
            }
        }
    }
}
