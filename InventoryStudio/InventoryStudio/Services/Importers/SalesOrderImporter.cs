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

        private Dictionary<string, string> salesOrderIndexToIdMapping = new Dictionary<string, string>();

        private Dictionary<string, string> salesOrderLineIndexToIdMapping = new Dictionary<string, string>();

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
                        ImportSalesOrder(companyId, data, salesOrderFields, userId, importResult, progressHandler, importTemplateID);
                    }
                    else if (dataSet.IndexOf(data) == 1) // Importing SalesOrderLine
                    {
                        ImportSalesOrderLine(companyId, data, salesOrderLineFields, userId, importResult, progressHandler, importTemplateID);
                    }
                    else if (dataSet.IndexOf(data) == 2) // Importing SalesOrderLineDetail
                    {
                        ImportSalesOrderLineDetail(companyId, data, salesOrderLineDetailFields, userId, importResult, progressHandler, importTemplateID);
                    }
                }
            }
            importResult.Update();
        }

        private void ImportSalesOrderLineDetail(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields, string userId, ImportResult importResult, FileHandlers.ProgressHandler progressHandler, string importTemplateID)
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
                            SetPropertyAsync(companyId, salesOrderLineDetail, field.Value, property);
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
            data.TryGetValue("SalesOrderLineIndex", out var salesOrderLineIndexStr);
            if (salesOrderLineIndexStr != null)
            {
                var salesOrderLineId = salesOrderLineIndexToIdMapping[salesOrderLineIndexStr];
                salesOrderLineDetail.SalesOrderLineID = salesOrderLineId;
            }
            salesOrderLineDetail.Create();
            importResult.SuccessfulRecords++;
            int progress = (int)((importResult.SuccessfulRecords / (double)importResult.TotalRecords) * 100);
            progressHandler?.Invoke(progress, importTemplateID);
        }

        private void ImportSalesOrderLine(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields, string userId, ImportResult importResult, FileHandlers.ProgressHandler progressHandler, string importTemplateID)
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
                            SetPropertyAsync(companyId, salesOrderLine, field.Value, property);
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
            data.TryGetValue("SalesOrderIndex", out var salesOrderIndexStr);
            if (salesOrderIndexStr != null)
            {
                var salesOrderId = salesOrderIndexToIdMapping[salesOrderIndexStr];
                salesOrderLine.SalesOrderID = salesOrderId;
            }
            salesOrderLine.CreatedBy = userId;
            salesOrderLine.CreatedOn = DateTime.Now;
            salesOrderLine.Create();
            importResult.SuccessfulRecords++;


            int progress = (int)((importResult.SuccessfulRecords / (double)importResult.TotalRecords) * 100);
            progressHandler?.Invoke(progress, importTemplateID);
        }

        private void ImportSalesOrder(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields, string userId, ImportResult importResult, FileHandlers.ProgressHandler progressHandler, string importTemplateID)
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
                            SetPropertyAsync(companyId, salesOrder, field.Value, property);
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
            data.TryGetValue("SalesOrderIndex", out var salesOrderIndexStr);
            if (salesOrderIndexStr != null)
                salesOrderIndexToIdMapping.Add(salesOrderIndexStr, salesOrder.SalesOrderID.ToString());

            int progress = (int)((importResult.SuccessfulRecords / (double)importResult.TotalRecords) * 100);
            progressHandler?.Invoke(progress, importTemplateID);
        }

        private void SetPropertyAsync(string companyId, SalesOrder salesOrder, string value, PropertyInfo property)
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
            else if (property.PropertyType == typeof(Customer))
            {
                CustomerFilter filter = new CustomerFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var customer = Customer.GetCustomer(companyId, filter);
                if (customer != null)
                    property.SetValue(salesOrder, customer);
            }
            else if (property.PropertyType == typeof(Location))
            {
                LocationFilter filter = new LocationFilter();
                filter.LocationName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.LocationName.SearchString = value;
                var location = Location.GetLocation(companyId, filter);
                if (location != null)
                    property.SetValue(salesOrder, location);
            }
            else if (property.PropertyType == typeof(Address) && (property.Name == "BillToAddress") || property.Name == "ShipToAddress")
            {
                AddressFilter filter = new AddressFilter();
                filter.Email = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Email.SearchString = value;
                var address = Address.GetAddress(companyId, filter);
                if (address != null)
                    property.SetValue(salesOrder, address);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(salesOrder, convertedValue);
            }

        }

        private void SetPropertyAsync(string companyId, SalesOrderLine salesOrderLine, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Location))
            {
                LocationFilter filter = new LocationFilter();
                filter.LocationName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.LocationName.SearchString = value;
                var location = Location.GetLocation(companyId, filter);
                if (location != null)
                    property.SetValue(salesOrderLine, location);
            }
            else if (property.PropertyType == typeof(Item))
            {
                var item = new Item(companyId, value);
                if (item != null)
                    property.SetValue(salesOrderLine, item);
            }
            else if (property.PropertyType == typeof(ItemUnit))
            {
                var filter = new ItemUnitFilter();
                filter.Name = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Name.SearchString = value;
                var itemUnits = ItemUnit.GetItemUnits(companyId, filter);
                if (itemUnits != null)
                    property.SetValue(salesOrderLine, itemUnits.FirstOrDefault());
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(salesOrderLine, convertedValue);
            }

        }

        private void SetPropertyAsync(string companyId, SalesOrderLineDetail salesOrderLineDetail, string value, PropertyInfo property)
        {
            if (property.PropertyType == typeof(Bin))
            {
                BinFilter filter = new BinFilter();
                filter.BinNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.BinNumber.SearchString = value;
                var bins = Bin.GetBins(companyId, filter);
                if (bins != null)
                    property.SetValue(salesOrderLineDetail, bins.FirstOrDefault());
            }
            else if (property.PropertyType == typeof(Inventory))
            {
                InventoryFilter filter = new InventoryFilter();
                filter.ItemID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ItemID.SearchString = value;
                var inventory = Inventory.GetInventory(filter);
                if (inventory != null)
                    property.SetValue(salesOrderLineDetail, inventory);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(salesOrderLineDetail, convertedValue);
            }
        }


    }
}
