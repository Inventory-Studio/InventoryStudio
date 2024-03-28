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


        private ImportResult importResult;

        public SalesOrderImporter() => importResult = new ImportResult();

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


            importResult.ImportTemplateID = importTemplateID;
            importResult.UploadBy = userId;
            importResult.UploadTime = DateTime.Now;
            importResult.TotalRecords = datas[0].Count;

            importResult.Create();
            var salesOrderAndIndexs = new List<(SalesOrder salesOrder, int index)>();
            var salesOrderLineAndIndexs = new List<(SalesOrderLine salesOrderLine, int salesOrderIndex, int salesOrderLineIndex)>();
            var salesOrderLineDetailAndIndexs = new List<(SalesOrderLineDetail salesOrderLineDetail, int index)>();


            foreach (var data in datas[0])
            {
                data.TryGetValue("SalesOrderIndex", out string? salesOrderIndex);
                var (salesOrder, error) = await GetEntity<SalesOrder>(companyId, data, salesOrderFields);
                if (!error && salesOrder != null)
                    salesOrderAndIndexs.Add((salesOrder, Convert.ToInt32(salesOrderIndex)));
            }

            foreach (var data in datas[1])
            {
                data.TryGetValue("SalesOrderIndex", out string? SalesOrderIndex);
                data.TryGetValue("SalesOrderLineIndex", out string? SalesOrderLineIndex);
                var (salesOrderLine, error) = await GetEntity<SalesOrderLine>(companyId, data, salesOrderLineFields);
                if (!error && salesOrderLine != null)
                    salesOrderLineAndIndexs.Add((salesOrderLine, Convert.ToInt32(SalesOrderIndex), Convert.ToInt32(SalesOrderLineIndex)));
            }

            foreach (var data in datas[2])
            {
                data.TryGetValue("SalesOrderLineIndex", out string? SalesOrderLineIndex);
                var (salesOrderLineDetail, error) = await GetEntity<SalesOrderLineDetail>(companyId, data, salesOrderLineDetailFields);
                if (!error && salesOrderLineDetail != null)
                    salesOrderLineDetailAndIndexs.Add((salesOrderLineDetail, Convert.ToInt32(SalesOrderLineIndex)));
            }

            foreach (var salesOrder in salesOrderAndIndexs)
            {
                var currentSalesOrderLines = salesOrderLineAndIndexs.Where(t => t.salesOrderIndex == salesOrder.index).ToList();
                if (currentSalesOrderLines.Any())
                {
                    foreach (var salesOrderLine in currentSalesOrderLines)
                    {
                        var currentSalesOrderLineDetails = salesOrderLineDetailAndIndexs.Where(t => t.index == salesOrderLine.salesOrderLineIndex).ToList();
                        if (currentSalesOrderLineDetails.Any())
                        {
                            salesOrderLine.salesOrderLine.SalesOrderLineDetails = currentSalesOrderLineDetails.Select(t => t.salesOrderLineDetail).ToList();
                        }
                    }
                    salesOrder.salesOrder.SalesOrderLines = currentSalesOrderLines.Select(t => t.salesOrderLine).ToList();
                }
                salesOrder.salesOrder.CreatedBy = userId;
                salesOrder.salesOrder.CompanyID = companyId;
                salesOrder.salesOrder.Status = SalesOrder.enumOrderStatus.Pending;
                try
                {
                    salesOrder.salesOrder.Create();
                    importResult.SuccessfulRecords++;
                }
                catch (Exception ex)
                {
                    var failedDataJson = JsonSerializer.Serialize(salesOrder.salesOrder);
                    var failedRecord = new ImportFailedRecord
                    {
                        ImportResultID = importResult.ImportResultID,
                        ErrorMessage = ex.Message,
                        FailedData = failedDataJson
                    };
                    failedRecord.Create();
                    importResult.FailedRecords++;
                }
                processedCount++;
                int progress = (int)(processedCount / (double)importResult.TotalRecords * 100);
                progressHandler?.Invoke(progress, importTemplateID);
            }
            importResult.Update();
        }


        private async Task<(T, bool)> GetEntity<T>(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields) where T : class, new()
        {
            var entity = new T();
            var error = false;
            try
            {
                foreach (var field in data)
                {
                    if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                    {

                        var property = typeof(T).GetProperty(destinationField);
                        if (property != null)
                        {

                            var convertedValue = await GetPropertyValueAsync(companyId, property, field.Value);

                            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                            {
                                var idProperty = typeof(T).GetProperty(property.Name + "ID");
                                if (idProperty != null)
                                {
                                    idProperty.SetValue(entity, convertedValue);
                                }
                                else
                                {
                                    throw new Exception($"ID property for {property.Name} not found.");
                                }
                            }
                            else
                            {
                                property.SetValue(entity, convertedValue);
                            }

                        }
                    }
                }
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
                error = true;
            }
            return (error ? default(T) : entity, error);
        }

        private async Task<object> GetPropertyValueAsync(string companyId, PropertyInfo property, string value)
        {
            Type propertyType = property.PropertyType;

            // Check if the property is nullable and get the underlying type
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            if (propertyType == typeof(decimal))
            {
                if (decimal.TryParse(value, out decimal decimalValue))
                {
                    return decimalValue;
                }
                else
                {
                    throw new Exception($"Unable to parse '{value}' to decimal.");
                }
            }
            else if (propertyType == typeof(int))
            {
                if (int.TryParse(value, out int intValue))
                {
                    return intValue;
                }
                else
                {
                    throw new Exception($"Unable to parse '{value}' to int.");
                }
            }
            else if (propertyType == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime dateTimeValue))
                {
                    return dateTimeValue;
                }
                else
                {
                    throw new Exception($"Unable to parse '{value}' to DateTime.");
                }
            }
            else if (property.Name == "Customer")
            {
                CustomerFilter filter = new CustomerFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var customer = Customer.GetCustomer(companyId, filter);
                if (customer != null)
                    return customer.CustomerID;
                else
                    throw new Exception($"Unable to find the customer {value}");
            }
            else if (property.Name == "BillToAddress" || property.Name == "ShipToAddress")
            {
                AddressFilter filter = new AddressFilter();
                filter.Email = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Email.SearchString = value;
                var address = Address.GetAddress(companyId, filter);
                if (address != null)
                    return address.AddressID;
            }
            else if (property.Name == "Location")
            {
                LocationFilter filter = new LocationFilter();
                filter.LocationName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.LocationName.SearchString = value;
                var location = Location.GetLocation(companyId, filter);
                if (location != null)
                    return location.LocationID;
            }
            else if (property.Name == "Bin")
            {
                BinFilter filter = new BinFilter();
                filter.BinNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.BinNumber.SearchString = value;
                var bins = Bin.GetBins(companyId, filter);
                if (bins != null)
                    return bins.First()?.BinID;
                else
                    throw new Exception($"Unable to find the Bin {value}");
            }
            else if (property.Name == "InventoryDetail")
            {
                InventoryDetailFilter filter = new InventoryDetailFilter();
                filter.InventoryNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.InventoryNumber.SearchString = value;
                var inventoryDetail = InventoryDetail.GetInventoryDetail(companyId, filter);
                if (inventoryDetail != null)
                    return inventoryDetail.InventoryDetailID;
            }
            else if (property.Name == "Item")
            {
                ItemFilter filter = new ItemFilter();
                filter.ItemNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ItemNumber.SearchString = value;
                var item = Item.GetItem(companyId, filter);
                if (item != null)
                    return item.ItemID;
            }
            else if (property.Name == "ItemUnit")
            {
                var filter = new ItemUnitFilter();
                filter.Name = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Name.SearchString = value;
                var itemUnits = ItemUnit.GetItemUnits(companyId, filter);
                if (itemUnits != null)
                    return itemUnits.First().ItemUnitID;
            }
            else if (property.Name == "InventoryDetail")
            {
                InventoryDetailFilter filter = new InventoryDetailFilter();
                filter.InventoryNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.InventoryNumber.SearchString = value;
                var inventoryDetail = InventoryDetail.GetInventoryDetail(companyId, filter);
                if (inventoryDetail != null)
                    return inventoryDetail.InventoryDetailID;
            }
            else
            {
                return Convert.ChangeType(value, propertyType);
            }
            return null;
        }

    }
}
