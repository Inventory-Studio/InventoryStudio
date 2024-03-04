using InventoryStudio.Services.File;
using ISLibrary.ImportTemplateManagement;
using ISLibrary;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace InventoryStudio.Importer
{
    public class ItemImporter
    {
        private readonly IFileParserFactory _fileParserFactory;

        public ItemImporter(IFileParserFactory fileParserFactory)
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
                var item = new Item();
                foreach (var field in data)
                {
                    if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                    {
                        var property = typeof(Item).GetProperty(destinationField);
                        if (property != null)
                        {
                            try
                            {
                                await SetPropertyAsync(companyId, item, field.Value, property);
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
                item.CreatedBy = userId;
                item.Create();
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

        private async System.Threading.Tasks.Task SetPropertyAsync(string companyId, Item item, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Item))
            {
                CompanyFilter filter = new CompanyFilter();
                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CompanyName.SearchString = value;
                var companies = Company.GetCompanies(filter);
                if (companies != null)
                    property.SetValue(item, companies.FirstOrDefault());
                else
                    throw new Exception($"Unable to find the Company {value}");
            }
            else if (property.PropertyType == typeof(Bin))
            {
                var filter = new BinFilter();
                filter.BinNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.BinNumber.SearchString = value;
                var clients = Bin.GetBins(companyId, filter);
                if (clients != null)
                    property.SetValue(item, clients.FirstOrDefault());
                else
                    throw new Exception($"Unable to find the Bin {value}");
            }
            else if (property.PropertyType == typeof(LabelProfile))
            {
                var filter = new LabelProfileFilter();
                filter.ProfileName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ProfileName.SearchString = value;
                var labelProfiles = LabelProfile.GetLabelProfiles(companyId, filter);
                if (labelProfiles != null)
                    property.SetValue(item, labelProfiles.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(ItemUnit) && (property.Name == "ItemUnitType" || property.Name == "PrimarySalesUnit" || property.Name == "PrimaryPurchaseUnit" || property.Name == "PrimaryStockUnit"))
            {
                var filter = new ItemUnitFilter();
                filter.Name = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Name.SearchString = value;
                var itemUnits = ItemUnit.GetItemUnits(companyId, filter);
                if (itemUnits != null)
                    property.SetValue(item, itemUnits.FirstOrDefault());
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(item, convertedValue);
            }

        }
    }
}
