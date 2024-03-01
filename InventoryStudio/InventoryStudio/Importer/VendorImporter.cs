using InventoryStudio.Services.File;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using ISLibrary.OrderManagement;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Reflection;

namespace InventoryStudio.Importer
{
    public class VendorImporter
    {
        private readonly IFileParserFactory _fileParserFactory;

        public VendorImporter(IFileParserFactory fileParserFactory)
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
                var vendor = new Vendor();
                foreach (var field in data)
                {
                    if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                    {
                        var property = typeof(Vendor).GetProperty(destinationField);
                        if (property != null)
                        {
                            try
                            {
                                await SetPropertyAsync(companyId, vendor, field.Value, property);
                            }
                            catch (Exception ex)
                            {
                                var failedRecord = new ImportFailedRecord
                                {
                                    ImportResultID = importResult.ImportResultID,
                                    ErrorMessage = ex.Message,
                                    FailedData = string.Join(",", data.Values)
                                };
                                failedRecord.Create();
                                importResult.FailedRecords++;
                            }
                        }
                    }
                }
                vendor.CreatedBy = userId;
                vendor.Create();
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

        private async System.Threading.Tasks.Task SetPropertyAsync(string companyId, Vendor vendor, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Company))
            {
                CompanyFilter filter = new CompanyFilter();
                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CompanyName.SearchString = value;
                var companies = Company.GetCompanies(filter);
                if (companies != null)
                    property.SetValue(vendor, companies.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(Client))
            {
                ClientFilter filter = new ClientFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var client = Client.GetClient(companyId, filter);
                if (client != null)
                    property.SetValue(vendor, client);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(vendor, convertedValue);
            }
        }
    }
}
