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
                TotalRecords = datas[0].Count,
            };
            importResult.Create();
            var salesOrders = new List<(SalesOrder, int)>();
            var salesOrderLines = new List<(SalesOrderLine, int)>();
            var salesOrderLineDetails = new List<(SalesOrderLineDetail, int)>();

            try
            {
                foreach (var data in datas[0]) // Importing SalesOrder
                {
                    data.TryGetValue("SalesOrderIndex", out string? salesOrderIndex);
                    var salesOrder = await GetEntity<SalesOrder>(companyId, data, salesOrderFields);
                    salesOrders.Add((salesOrder, Convert.ToInt32(salesOrderIndex)));
                }

                foreach (var data in datas[1]) // Importing SalesOrderLine
                {
                    var SalesOrderIndex = data["SalesOrderIndex"];
                    var salesOrderLine = await GetEntity<SalesOrderLine>(companyId, data, salesOrderLineFields);
                    salesOrderLines.Add((salesOrderLine, Convert.ToInt32(SalesOrderIndex)));
                }

                foreach (var data in datas[2]) // Importing SalesOrderLineDetail
                {
                    var SalesOrderLineIndex = data["SalesOrderLineIndex"];
                    var salesOrderLineDetail = await GetEntity<SalesOrderLineDetail>(companyId, data, salesOrderLineDetailFields);
                    salesOrderLineDetails.Add((salesOrderLineDetail, Convert.ToInt32(SalesOrderLineIndex)));
                }

                foreach (var salesOrder in salesOrders)
                {
                    var currentSalesOrderLines = salesOrderLines.Where(t => t.Item2 == salesOrder.Item2).ToList();
                    if (currentSalesOrderLines.Any())
                    {
                        foreach (var salesOrderLine in currentSalesOrderLines)
                        {
                            var currentSalesOrderLineDetails = salesOrderLineDetails.Where(t => t.Item2 == salesOrderLine.Item2).ToList();
                            if (currentSalesOrderLineDetails.Any())
                            {
                                salesOrderLine.Item1.SalesOrderLineDetails = currentSalesOrderLineDetails.Select(t => t.Item1).ToList();
                            }
                        }

                        salesOrder.Item1.SalesOrderLines = currentSalesOrderLines.Select(t => t.Item1).ToList();
                    }
                    salesOrder.Item1.CreatedBy = userId;
                    salesOrder.Item1.CompanyID = companyId;
                    salesOrder.Item1.Status = SalesOrder.enumOrderStatus.Pending;
                    salesOrder.Item1.Create();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private async Task<T> GetEntity<T>(string companyId, Dictionary<string, string> data, List<ImportTemplateField> importTemplateFields) where T : class, new()
        {
            var entity = new T();

            foreach (var field in data)
            {
                if (TryGetDestinationField(field.Key, importTemplateFields, out var destinationField))
                {
                    var property = typeof(T).GetProperty(destinationField);
                    if (property != null)
                    {
                        try
                        {
                            var convertedValue = await GetPropertyValueAsync(companyId, property, field.Value);
                            property.SetValue(entity, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            return entity;
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
            else if (propertyType == typeof(Customer))
            {
                CustomerFilter filter = new CustomerFilter();
                filter.EmailAddress = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.EmailAddress.SearchString = value;
                var customer = Customer.GetCustomer(companyId, filter);
                if (customer != null)
                    return customer;
                else
                    throw new Exception($"Unable to find the customer {value}");
            }
            else if (propertyType == typeof(Address) && (property.Name == "BillToAddress") || property.Name == "ShipToAddress")
            {
                AddressFilter filter = new AddressFilter();
                filter.Email = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Email.SearchString = value;
                var address = Address.GetAddress(companyId, filter);
                if (address != null)
                    return address;
            }
            else if (property.PropertyType == typeof(Location))
            {
                LocationFilter filter = new LocationFilter();
                filter.LocationName = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.LocationName.SearchString = value;
                var location = Location.GetLocation(companyId, filter);
                if (location != null)
                    return location;
            }
            else if (propertyType == typeof(Bin))
            {
                BinFilter filter = new BinFilter();
                filter.BinNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.BinNumber.SearchString = value;
                var bins = Bin.GetBins(companyId, filter);
                if (bins != null)
                    return bins.First();
                else
                    throw new Exception($"Unable to find the Bin {value}");
            }
            else if (property.PropertyType == typeof(Inventory))
            {
                InventoryFilter filter = new InventoryFilter();
                filter.ItemID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ItemID.SearchString = value;
                var inventory = Inventory.GetInventory(filter);
                if (inventory != null)
                    return inventory;
            }
            else if (property.PropertyType == typeof(Item))
            {
                ItemFilter filter = new ItemFilter();
                filter.ItemNumber = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ItemNumber.SearchString = value;
                var item = Item.GetItem(companyId, filter);
                if (item != null)
                    return item;
            }
            else if (propertyType == typeof(ItemUnit))
            {
                var filter = new ItemUnitFilter();
                filter.Name = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Name.SearchString = value;
                var itemUnits = ItemUnit.GetItemUnits(companyId, filter);
                if (itemUnits != null)
                    return itemUnits.First();
            }
            else
            {
                return Convert.ChangeType(value, propertyType);
            }
            return null;
        }

    }
}
