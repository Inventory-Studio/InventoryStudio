using ICSharpCode.SharpZipLib.Core;
using InventoryStudio.Models;
using InventoryStudio.Services.FileHandlers;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using ISLibrary.OrderManagement;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.Crypt.Dsig;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text.Json;

namespace InventoryStudio.Services.Importers
{
    public class SalesOrderImporter : BaseImporter, IImporter, IMultipleLineImporter
    {
        public Task ImportDataAsync(string companyId, string importTemplateID, string userId, FileHandlers.ProgressHandler progressHandler, List<Dictionary<string, string>> datas)
        {
            throw new NotImplementedException();
        }


        private List<ImportFailedRecord> failedRecords = new List<ImportFailedRecord>();

        public async Task ImportDatasAsync(string companyId, string importTemplateID, string userId, FileHandlers.ProgressHandler progressHandler, List<List<Dictionary<string, string>>> datas)
        {
            var importTemplateFilter = new ImportTemplateFieldFilter();
            importTemplateFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            importTemplateFilter.ImportTemplateID.SearchString = importTemplateID;
            var importTemplateFields = ImportTemplateField.GetImportTemplateFields(companyId, importTemplateFilter);
            int processedCount = 0;
            var salesOrderFields = importTemplateFields.Where(t => t.DestinationTable == "SalesOrder").ToList();
            var salesOrderLineFields = importTemplateFields.Where(t => t.DestinationTable == "SalesOrderLine").ToList();
            var salesOrderLineDetailFields = importTemplateFields.Where(t => t.DestinationTable == "SalesOrderLineDetail").ToList();
            var importResult = new ImportResult
            {
                ImportTemplateID = importTemplateID,
                UploadBy = userId,
                UploadTime = DateTime.Now,
                TotalRecords = datas.Count,
            };
            importResult.Create();
            foreach (var dataSet in datas)
            {
                foreach (var data in dataSet)
                {
                    if (dataSet.IndexOf(data) == 0) // Importing SalesOrder
                    {
                        await ImportSalesOrder(companyId, data, salesOrderFields, userId, importResult, progressHandler, importTemplateID);
                    }
                    else if (dataSet.IndexOf(data) == 1) // Importing SalesOrderLine
                    {
                        await ImportSalesOrderLine(companyId, data, salesOrderLineFields, userId, importResult, progressHandler, importTemplateID);
                    }
                    else if (dataSet.IndexOf(data) == 2) // Importing SalesOrderLineDetail
                    {
                        await ImportSalesOrderLineDetail(companyId, data, salesOrderLineDetailFields, userId, importResult, progressHandler, importTemplateID);
                    }
                }
            }
            importResult.Update();
        }

        private async Task ImportSalesOrderLineDetail(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields, string userId, ImportResult importResult, FileHandlers.ProgressHandler progressHandler, string importTemplateID)
        {
            var salesOrderLineDetail = new SalesOrderLineDetail();
            foreach (var field in data)
            {
                if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                {
                    var property = typeof(SalesOrder).GetProperty(destinationField);
                    if (property != null)
                    {
                        try
                        {
                            await SetPropertyAsync(companyId, salesOrderLineDetail, field.Value, property);
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
                            importResult.FailedRecords++;
                            failedRecords.Add(failedRecord);
                        }
                    }
                }
            }

            salesOrderLineDetail.CreatedBy = userId;
            salesOrderLineDetail.CreatedOn = DateTime.Now;
            importResult.SuccessfulRecords++;
            salesOrderLineDetail.Create();
            int progress = (int)((importResult.SuccessfulRecords / (double)importResult.TotalRecords) * 100);
            progressHandler?.Invoke(progress, importTemplateID);
        }

        private async Task ImportSalesOrderLine(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields, string userId, ImportResult importResult, FileHandlers.ProgressHandler progressHandler, string importTemplateID)
        {
            var salesOrderLine = new SalesOrderLine();
            foreach (var field in data)
            {
                if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                {
                    var property = typeof(SalesOrderLine).GetProperty(destinationField);
                    if (property != null)
                    {
                        try
                        {
                            await SetPropertyAsync(companyId, salesOrderLine, field.Value, property);
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
                            importResult.FailedRecords++;
                            failedRecords.Add(failedRecord);
                        }
                    }
                }
            }
            salesOrderLine.CreatedBy = userId;
            salesOrderLine.CreatedOn = DateTime.Now;
            salesOrderLine.Create();
            importResult.SuccessfulRecords++;

            int progress = (int)((importResult.SuccessfulRecords / (double)importResult.TotalRecords) * 100);
            progressHandler?.Invoke(progress, importTemplateID);
        }

        private async Task ImportSalesOrder(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields, string userId, ImportResult importResult, FileHandlers.ProgressHandler progressHandler, string importTemplateID)
        {
            var salesOrder = new SalesOrder();

            foreach (var field in data)
            {
                if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                {
                    var property = typeof(SalesOrder).GetProperty(destinationField);
                    if (property != null)
                    {
                        try
                        {
                            await SetPropertyAsync(companyId, salesOrder, field.Value, property);
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
                            importResult.FailedRecords++;
                            failedRecords.Add(failedRecord);
                        }
                    }
                }
            }

            salesOrder.CreatedBy = userId;
            salesOrder.CreatedOn = DateTime.Now;
            salesOrder.Create();
            importResult.SuccessfulRecords++;

            int progress = (int)((importResult.SuccessfulRecords / (double)importResult.TotalRecords) * 100);
            progressHandler?.Invoke(progress, importTemplateID);
        }

        private async Task SetPropertyAsync(string companyId, SalesOrder salesOrder, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Company))
            {
                CompanyFilter filter = new CompanyFilter();
                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CompanyName.SearchString = value;
                var companies = Company.GetCompanies(filter);
                if (companies != null)
                    property.SetValue(salesOrder, companies.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(Client))
            {
                ClientFilter filter = new ClientFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var client = Client.GetClient(companyId, filter);
                if (client != null)
                    property.SetValue(salesOrder, client);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(salesOrder, convertedValue);
            }
        }

        private async System.Threading.Tasks.Task SetPropertyAsync(string companyId, SalesOrderLine salesOrderLine, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Company))
            {
                CompanyFilter filter = new CompanyFilter();
                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CompanyName.SearchString = value;
                var companies = Company.GetCompanies(filter);
                if (companies != null)
                    property.SetValue(salesOrderLine, companies.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(Client))
            {
                ClientFilter filter = new ClientFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var client = Client.GetClient(companyId, filter);
                if (client != null)
                    property.SetValue(salesOrderLine, client);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(salesOrderLine, convertedValue);
            }
        }

        private async System.Threading.Tasks.Task SetPropertyAsync(string companyId, SalesOrderLineDetail salesOrderLineDetail, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Company))
            {
                CompanyFilter filter = new CompanyFilter();
                filter.CompanyName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.CompanyName.SearchString = value;
                var companies = Company.GetCompanies(filter);
                if (companies != null)
                    property.SetValue(salesOrderLineDetail, companies.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(Client))
            {
                ClientFilter filter = new ClientFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var client = Client.GetClient(companyId, filter);
                if (client != null)
                    property.SetValue(salesOrderLineDetail, client);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(salesOrderLineDetail, convertedValue);
            }
        }
    }
}
