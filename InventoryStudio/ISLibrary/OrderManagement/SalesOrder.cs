using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ISLibrary.Item;

namespace ISLibrary.OrderManagement
{
    public class SalesOrder : BaseClass
    {
        public string SalesOrderID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(SalesOrderID); } }

        public string CompanyID { get; set; }

        public string CustomerID { get; set; }

        public string PONumber { get; set; }

        public DateTime TranDate { get; set; }

        public string? LocationID { get; set; }

        public string? BillToAddressID { get; set; }

        public string ShipToAddressID { get; set; }

        public decimal? ShippingAmount { get; set; }

        public decimal? ShippingTaxAmount { get; set; }

        public decimal? ItemTaxAmount { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string? SalesSource { get; set; }

        public string? ShippingMethod { get; set; }

        public string? ShippingCarrier { get; set; }

        public string? ShippingPackage { get; set; }

        public string? ShippingServiceCode { get; set; }

        public DateTime? ShipFrom { get; set; }

        public DateTime? ShipTo { get; set; }

        public enum enumOrderStatus
        {
            [Description("Pending Approval")]
            Pending,
            [Description("Pending Fulfillment")]
            PendingFulfillment,
            [Description("Partially Shipped")]
            PartiallyShipped,
            [Description("Shipped")]
            Shipped
        }

        public enumOrderStatus Status { get; set; }

        public bool IsClosed { get; set; }

        public string? ExternalID { get; set; }

        public string? InternalNote { get; set; }

        public string? CustomerNote { get; set; }

        public string? GiftMessage { get; set; }

        public string? LabelMessage { get; set; }

        public string? ReferenceNumber { get; set; }

        public bool SignatureRequired { get; set; }

        public string? ShopifyOrderID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        private List<SalesOrderLine> mSalesOrderLine = null;

        public List<SalesOrderLine> SalesOrderLines
        {
            get
            {
                SalesOrderLineFilter objFilter = null;
                {
                    try
                    {
                        if (mSalesOrderLine == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(SalesOrderID))
                        {
                            objFilter = new SalesOrderLineFilter();
                            objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.SalesOrderID.SearchString = SalesOrderID;
                            mSalesOrderLine = SalesOrderLine.GetSalesOrderLines(CompanyID, objFilter);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                    return mSalesOrderLine;
                }
            }
            set
            {
                mSalesOrderLine = value;
            }
        }

        private Address mBillShippToAddress = null;

        public Address BillToAddress
        {
            get
            {
                AddressFilter objFilter = null;
                {
                    try
                    {
                        if (mBillShippToAddress == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(BillToAddressID) && !string.IsNullOrEmpty(SalesOrderID))
                        {
                            objFilter = new AddressFilter();
                            objFilter.AddressID = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.AddressID.SearchString = BillToAddressID;
                            mBillShippToAddress = Address.GetAddress(CompanyID, objFilter);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                    return mBillShippToAddress;
                }
            }
            set { mBillShippToAddress = value; }
        }

        private Address mShipToAddress = null;

        public Address ShipToAddress
        {
            get
            {
                AddressFilter objFilter = null;
                {
                    try
                    {
                        if (mShipToAddress == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ShipToAddressID) && !string.IsNullOrEmpty(SalesOrderID))
                        {
                            objFilter = new AddressFilter();
                            objFilter.AddressID = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.AddressID.SearchString = ShipToAddressID;
                            mShipToAddress = Address.GetAddress(CompanyID, objFilter);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                    return mShipToAddress;
                }
            }
            set { mShipToAddress = value; }
        }

        private Customer mCustomer = null;

        public Customer Customer
        {
            get
            {
                CustomerFilter objFilter = null;
                {
                    try
                    {
                        if (mCustomer == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(SalesOrderID))
                        {
                            objFilter = new CustomerFilter();
                            objFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.CustomerID.SearchString = CustomerID;
                            mCustomer = Customer.GetCustomer(CompanyID, objFilter);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                    return mCustomer;
                }
            }
            set { mCustomer = value; }
        }



        public SalesOrder()
        {

        }

        public SalesOrder(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public SalesOrder(string CompanyID, string SalesOrderID)
        {
            this.CompanyID = CompanyID;
            this.SalesOrderID = SalesOrderID;
            Load();
        }

        public SalesOrder(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("CustomerID")) CustomerID = Convert.ToString(objRow["CustomerID"]);
                if (objColumns.Contains("PONumber")) PONumber = Convert.ToString(objRow["PONumber"]);
                if (objColumns.Contains("TranDate")) TranDate = Convert.ToDateTime(objRow["TranDate"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("BillToAddressID")) BillToAddressID = Convert.ToString(objRow["BillToAddressID"]);
                if (objColumns.Contains("ShipToAddressID")) ShipToAddressID = Convert.ToString(objRow["ShipToAddressID"]);
                if (objColumns.Contains("ShippingAmount") && objRow["ShippingAmount"] != DBNull.Value) ShippingAmount = Convert.ToDecimal(objRow["ShippingAmount"]);
                if (objColumns.Contains("ShippingTaxAmount") && objRow["ShippingTaxAmount"] != DBNull.Value) ShippingTaxAmount = Convert.ToDecimal(objRow["ShippingTaxAmount"]);
                if (objColumns.Contains("ItemTaxAmount") && objRow["ItemTaxAmount"] != DBNull.Value) ItemTaxAmount = Convert.ToDecimal(objRow["ItemTaxAmount"]);
                if (objColumns.Contains("DiscountAmount") && objRow["DiscountAmount"] != DBNull.Value) DiscountAmount = Convert.ToDecimal(objRow["DiscountAmount"]);
                if (objColumns.Contains("SalesSource")) SalesSource = Convert.ToString(objRow["SalesSource"]);
                if (objColumns.Contains("ShippingMethod")) ShippingMethod = Convert.ToString(objRow["ShippingMethod"]);
                if (objColumns.Contains("ShippingCarrier")) ShippingCarrier = Convert.ToString(objRow["ShippingCarrier"]);
                if (objColumns.Contains("ShippingPackage")) ShippingPackage = Convert.ToString(objRow["ShippingPackage"]);
                if (objColumns.Contains("ShippingServiceCode")) ShippingServiceCode = Convert.ToString(objRow["ShippingServiceCode"]);
                if (objColumns.Contains("ShipFrom") && objRow["ShipFrom"] != DBNull.Value) ShipFrom = Convert.ToDateTime(objRow["ShipFrom"]);
                if (objColumns.Contains("ShipTo") && objRow["ShipTo"] != DBNull.Value) ShipTo = Convert.ToDateTime(objRow["ShipTo"]);
                if (objColumns.Contains("Status") && objRow["Status"] != DBNull.Value && Enum.TryParse(Convert.ToString(objRow["Status"]), out enumOrderStatus orderStatus)) Status = orderStatus;
                if (objColumns.Contains("IsClosed")) IsClosed = Convert.ToBoolean(objRow["IsClosed"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("InternalNote")) InternalNote = Convert.ToString(objRow["InternalNote"]);
                if (objColumns.Contains("CustomerNote")) CustomerNote = Convert.ToString(objRow["CustomerNote"]);
                if (objColumns.Contains("GiftMessage")) GiftMessage = Convert.ToString(objRow["GiftMessage"]);
                if (objColumns.Contains("LabelMessage")) LabelMessage = Convert.ToString(objRow["LabelMessage"]);
                if (objColumns.Contains("ReferenceNumber")) ReferenceNumber = Convert.ToString(objRow["ReferenceNumber"]);
                if (objColumns.Contains("SignatureRequired")) SignatureRequired = Convert.ToBoolean(objRow["SignatureRequired"]);
                if (objColumns.Contains("ShopifyOrderID")) ShopifyOrderID = Convert.ToString(objRow["ShopifyOrderID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("Missing SalesOrderID in the datarow");
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
            base.Load();
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.* " +
                         "FROM SalesOrder s (NOLOCK) " +
                         "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND s.SalesOrderID = " + Database.HandleQuote(SalesOrderID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SalesOrder is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }

        }

        public override bool Create()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Create(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(PONumber)) throw new Exception("PONumber is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, SalesOrder already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");



                if (Customer != null)
                {
                    Customer.CompanyID = CompanyID;
                    Customer.CreatedBy = CreatedBy;
                    Customer.Create(objConn, objTran);
                    CustomerID = Customer.CustomerID;
                }

                if (ShipToAddress != null)
                {
                    ShipToAddress.CompanyID = CompanyID;
                    ShipToAddress.CreatedBy = CreatedBy;
                    ShipToAddress.Create(objConn, objTran);
                    ShipToAddressID = ShipToAddress.AddressID;
                }

                if (BillToAddress != null)
                {
                    BillToAddress.CompanyID = CompanyID;
                    BillToAddress.CreatedBy = CreatedBy;
                    BillToAddress.Create(objConn, objTran);
                    BillToAddressID = BillToAddress.AddressID;
                }

                dicParam["CompanyID"] = CompanyID;
                dicParam["CustomerID"] = CustomerID;
                dicParam["PONumber"] = PONumber;
                dicParam["TranDate"] = TranDate;
                dicParam["LocationID"] = LocationID;
                dicParam["BillToAddressID"] = BillToAddressID;
                dicParam["ShipToAddressID"] = ShipToAddressID;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["ShippingTaxAmount"] = ShippingTaxAmount;
                dicParam["ItemTaxAmount"] = ItemTaxAmount;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["SalesSource"] = SalesSource;
                dicParam["ShippingMethod"] = ShippingMethod;
                dicParam["ShippingCarrier"] = ShippingCarrier;
                dicParam["ShippingPackage"] = ShippingPackage;
                dicParam["ShippingServiceCode"] = ShippingServiceCode;
                dicParam["ShipFrom"] = ShipFrom;
                dicParam["ShipTo"] = ShipTo;
                dicParam["Status"] = enumOrderStatus.Pending;
                dicParam["IsClosed"] = IsClosed;
                dicParam["ExternalID"] = ExternalID;
                dicParam["InternalNote"] = InternalNote;
                dicParam["CustomerNote"] = CustomerNote;
                dicParam["GiftMessage"] = GiftMessage;
                dicParam["LabelMessage"] = LabelMessage;
                dicParam["ReferenceNumber"] = ReferenceNumber;
                dicParam["SignatureRequired"] = SignatureRequired;
                dicParam["ShopifyOrderID"] = ShopifyOrderID;
                dicParam["CreatedOn"] = DateTime.Now;
                dicParam["CreatedBy"] = CreatedBy;
                SalesOrderID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SalesOrder"), objConn, objTran).ToString();



                if (SalesOrderLines != null)
                {
                    foreach (var objSalesOrderLine in SalesOrderLines)
                    {
                        if (objSalesOrderLine.IsNew)
                        {
                            objSalesOrderLine.SalesOrderID = SalesOrderID;
                            objSalesOrderLine.CompanyID = CompanyID;
                            objSalesOrderLine.CreatedBy = CreatedBy;
                            objSalesOrderLine.Create(objConn, objTran);
                        }
                    }
                }
                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
            }
            LogAuditData(enumActionType.Create);
            return true;
        }

        public override bool Update()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Update(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(PONumber)) throw new Exception("PONumber is required");
                if (IsNew) throw new Exception("Update cannot be performed, SalesOrderID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["CustomerID"] = CustomerID;
                dicParam["PONumber"] = PONumber;
                dicParam["TranDate"] = TranDate;
                dicParam["LocationID"] = LocationID;
                dicParam["BillToAddressID"] = BillToAddressID;
                dicParam["ShipToAddressID"] = ShipToAddressID;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["ShippingTaxAmount"] = ShippingTaxAmount;
                dicParam["ItemTaxAmount"] = ItemTaxAmount;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["SalesSource"] = SalesSource;
                dicParam["ShippingMethod"] = ShippingMethod;
                dicParam["ShippingCarrier"] = ShippingCarrier;
                dicParam["ShippingPackage"] = ShippingPackage;
                dicParam["ShippingServiceCode"] = ShippingServiceCode;
                dicParam["ShipFrom"] = ShipFrom;
                dicParam["ShipTo"] = ShipTo;
                dicParam["Status"] = Status;
                dicParam["IsClosed"] = IsClosed;
                dicParam["ExternalID"] = ExternalID;
                dicParam["InternalNote"] = InternalNote;
                dicParam["CustomerNote"] = CustomerNote;
                dicParam["GiftMessage"] = GiftMessage;
                dicParam["LabelMessage"] = LabelMessage;
                dicParam["ReferenceNumber"] = ReferenceNumber;
                dicParam["SignatureRequired"] = SignatureRequired;
                dicParam["ShopifyOrderID"] = ShopifyOrderID;

                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["SalesOrderID"] = SalesOrderID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SalesOrder"), objConn, objTran);


                if (Customer != null)
                {
                    Customer.UpdatedBy = UpdatedBy;
                    Customer.Update(objConn, objTran);
                }
                if (ShipToAddress != null)
                {
                    ShipToAddress.UpdatedBy = UpdatedBy;
                    ShipToAddress.Update(objConn, objTran);
                }
                if (BillToAddress != null)
                {
                    BillToAddress.UpdatedBy = UpdatedBy;
                    BillToAddress.Update(objConn, objTran);
                }

                if (SalesOrderLines != null)
                {
                    // Get the current SalesOrderLines from the database
                    var filter = new SalesOrderLineFilter();
                    filter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                    filter.SalesOrderID.SearchString = SalesOrderID;
                    List<SalesOrderLine> currentSalesOrderLines = SalesOrderLine.GetSalesOrderLines(CompanyID, filter);

                    // Update or create SalesOrderLines
                    foreach (var objSalesOrderLine in SalesOrderLines)
                    {
                        if (objSalesOrderLine.IsNew)
                        {
                            if (Status == enumOrderStatus.PendingFulfillment)
                            {
                                UpdateInventory(objSalesOrderLine);

                            }
                            objSalesOrderLine.SalesOrderID = SalesOrderID;
                            objSalesOrderLine.CompanyID = CompanyID;
                            objSalesOrderLine.CreatedBy = UpdatedBy;
                            objSalesOrderLine.Create(objConn, objTran);
                        }
                        else
                        {
                            if (Status == enumOrderStatus.PendingFulfillment)
                            {
                                if (objSalesOrderLine.QuantityCommitted == 0)
                                {
                                    UpdateInventory(objSalesOrderLine);
                                }
                                else
                                {
                                    SalesOrderLine objExistingSalesOrderLine = new SalesOrderLine(CompanyID, objSalesOrderLine.SalesOrderLineID);
                                    if (objExistingSalesOrderLine.Quantity != objSalesOrderLine.Quantity)
                                    {
                                        objSalesOrderLine.Inventory.QtyAvailable += objExistingSalesOrderLine.QuantityCommitted.Value;
                                        objSalesOrderLine.Inventory.QtyCommitted -= objExistingSalesOrderLine.QuantityCommitted.Value;
                                        objSalesOrderLine.Inventory.QtyBackOrdered -= objExistingSalesOrderLine.QuantityBackOrder;

                                        UpdateInventory(objSalesOrderLine);
                                        foreach (var objSalesOrderLineDetail in objExistingSalesOrderLine.SalesOrderLineDetails)
                                        {
                                            objSalesOrderLineDetail.InventoryDetail.Available += objSalesOrderLineDetail.Quantity;
                                            objSalesOrderLineDetail.InventoryDetail.Update();
                                        }
                                    }
                                }

                            }



                            objSalesOrderLine.UpdatedBy = UpdatedBy;
                            objSalesOrderLine.Update(objConn, objTran);
                        }
                    }

                    // Delete SalesOrderLines that are not in the current SalesOrderLines
                    foreach (var currentSalesOrderLine in currentSalesOrderLines)
                    {
                        if (!SalesOrderLines.Exists(t => t.SalesOrderLineID == currentSalesOrderLine.SalesOrderLineID))
                        {
                            if (Status == enumOrderStatus.PendingFulfillment)
                            {
                                currentSalesOrderLine.Inventory.QtyAvailable += currentSalesOrderLine.QuantityCommitted.Value;
                                currentSalesOrderLine.Inventory.QtyCommitted -= currentSalesOrderLine.QuantityCommitted.Value;
                                currentSalesOrderLine.Inventory.QtyBackOrdered -= currentSalesOrderLine.QuantityBackOrder;
                                currentSalesOrderLine.Inventory.Update();
                                foreach (var objSalesOrderLineDetail in currentSalesOrderLine.SalesOrderLineDetails)
                                {
                                    objSalesOrderLineDetail.InventoryDetail.Available += objSalesOrderLineDetail.Quantity;
                                    objSalesOrderLineDetail.InventoryDetail.Update();

                                    //Inventory Log
                                    InventoryLog objInventoryLog = new InventoryLog();
                                    objInventoryLog.ItemID = objSalesOrderLineDetail.ItemID;
                                    objInventoryLog.CompanyID = CompanyID;
                                    objInventoryLog.ChangeType = "SalesOrder";
                                    objInventoryLog.ChangeQuantity = objSalesOrderLineDetail.Quantity;
                                    objInventoryLog.ParentObjectID = SalesOrderID;
                                    objInventoryLog.BinID = objSalesOrderLineDetail.InventoryDetail.BinID;
                                    objInventoryLog.InventoryDetailID = objSalesOrderLineDetail.InventoryDetail.InventoryDetailID;
                                    objInventoryLog.InventoryNumber = objSalesOrderLineDetail.InventoryDetail.InventoryNumber;
                                    objInventoryLog.CreatedBy = CreatedBy;
                                    objInventoryLog.Create();
                                }

                            }
                            currentSalesOrderLine.Delete(objConn, objTran);
                        }
                    }
                }




                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                dicWParam = null;
            }

            LogAuditData(enumActionType.Update);
            return true;
        }

        public void UpdateInventory(SalesOrderLine objSalesOrderLine)
        {
            if (objSalesOrderLine.Quantity > objSalesOrderLine.Inventory.QtyAvailable)
            {
                objSalesOrderLine.QuantityBackOrder = objSalesOrderLine.Quantity - objSalesOrderLine.Inventory.QtyAvailable;
                objSalesOrderLine.QuantityCommitted = objSalesOrderLine.Inventory.QtyAvailable;

                objSalesOrderLine.Inventory.QtyBackOrdered += objSalesOrderLine.QuantityBackOrder;
            }
            else
            {
                objSalesOrderLine.QuantityCommitted = objSalesOrderLine.Quantity;
            }
            objSalesOrderLine.QuantityOnHand = objSalesOrderLine.Inventory.QtyOnHand;
            objSalesOrderLine.QuantityAvailable = objSalesOrderLine.Inventory.QtyAvailable - objSalesOrderLine.QuantityCommitted.Value;
            objSalesOrderLine.Inventory.QtyCommitted += objSalesOrderLine.QuantityCommitted.Value;
            objSalesOrderLine.Inventory.QtyAvailable = objSalesOrderLine.QuantityAvailable;
            objSalesOrderLine.Inventory.Update();

            foreach (var objSalesOrderLineDetail in objSalesOrderLine.SalesOrderLineDetails)
            {
                objSalesOrderLineDetail.InventoryDetail.Available -= objSalesOrderLineDetail.Quantity;
                objSalesOrderLineDetail.InventoryDetail.Update();

                //Inventory Log
                InventoryLog objInventoryLog = new InventoryLog();
                objInventoryLog.ItemID = objSalesOrderLineDetail.ItemID;
                objInventoryLog.CompanyID = CompanyID;
                objInventoryLog.ChangeType = "SalesOrder";
                objInventoryLog.ChangeQuantity = -objSalesOrderLineDetail.Quantity;
                objInventoryLog.ParentObjectID = SalesOrderID;
                objInventoryLog.BinID = objSalesOrderLineDetail.InventoryDetail.BinID;
                objInventoryLog.InventoryDetailID = objSalesOrderLineDetail.InventoryDetail.InventoryDetailID;
                objInventoryLog.InventoryNumber = objSalesOrderLineDetail.InventoryDetail.InventoryNumber;
                objInventoryLog.CreatedBy = CreatedBy;
                objInventoryLog.Create();
            }
        }


        public bool Check(out string errorMessage)
        {
            var salesOrder = this;
            errorMessage = string.Empty;
            var isError = false;
            var loctionIds = salesOrder.SalesOrderLines.Select(t => t.LocationID).ToList();
            if (loctionIds.Any())
            {
                foreach (var locationId in loctionIds)
                {
                    var location = new ISLibrary.Location(CompanyID, locationId);
                    if (location == null)
                    {
                        errorMessage += $"Location with ID {locationId} not found";
                        isError = true;
                    }
                }
            }
            var itemIds = salesOrder.SalesOrderLines.Select(t => t.ItemID).ToList();
            if (itemIds.Any())
            {
                foreach (var itemId in itemIds)
                {
                    var item = new Item(CompanyID, itemId);
                    if (item == null)
                    {
                        errorMessage += Environment.NewLine;
                        errorMessage += $"Item with ID {itemId} not found";
                        isError = true;
                    }
                }
            }

            var itemUnitIds = salesOrder.SalesOrderLines.Select(t => t.ItemUnitID).ToList();
            if (itemUnitIds.Any())
            {
                foreach (var itemUnitId in itemUnitIds)
                {
                    var itemUnit = new ItemUnit(itemUnitId);
                    if (itemUnit == null)
                    {
                        errorMessage += Environment.NewLine;
                        errorMessage += $"ItemUnit with ID {itemUnitId} not found";
                        isError = true;
                    }
                }
            }

            var binIds = salesOrder.SalesOrderLines.SelectMany(line => line.SalesOrderLineDetails)
            .Select(detail => detail.BinID).ToList();
            if (binIds.Any())
            {
                foreach (var binId in binIds)
                {
                    var bin = new Bin(CompanyID, binId);
                    if (bin == null)
                    {
                        errorMessage += Environment.NewLine;
                        errorMessage += $"Bin with ID {binId} not found";
                        isError = true;
                    }
                }
            }

            var inventoryDetailIds = salesOrder.SalesOrderLines.SelectMany(line => line.SalesOrderLineDetails)
            .Select(detail => detail.InventoryDetailID).ToList();
            if (inventoryDetailIds.Any())
            {
                foreach (var inventoryDetailId in inventoryDetailIds)
                {
                    var inventoryDetail = new InventoryDetail(inventoryDetailId);
                    if (inventoryDetail == null)
                    {
                        errorMessage += Environment.NewLine;
                        errorMessage += $"InventoryDetail with ID {inventoryDetailId} not found";
                        isError = true;
                    }
                }
            }
            return isError;
        }
        public override bool Delete()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Delete(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            if (!IsLoaded) Load();

            base.Delete();

            Hashtable dicDParam = new Hashtable();
            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, SalesOrderID is missing");
                dicDParam["SalesOrderID"] = SalesOrderID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SalesOrder"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            LogAuditData(enumActionType.Delete);
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;
            strSQL = "SELECT TOP 1 p.* " +
                     "FROM SalesOrder (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.PONumber=" + Database.HandleQuote(PONumber);
            return Database.HasRows(strSQL);
        }

        public static SalesOrder GetSalesOrder(string CompanyID, SalesOrderFilter Filter)
        {
            List<SalesOrder> objSalesOrders = null;
            SalesOrder objReturn = null;

            try
            {
                objSalesOrders = GetSalesOrders(CompanyID, Filter);
                if (objSalesOrders != null && objSalesOrders.Count >= 1) objReturn = objSalesOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrders = null;
            }
            return objReturn;
        }

        public static List<SalesOrder> GetSalesOrders(string CompanyID)
        {
            int intTotalCount = 0;
            return GetSalesOrders(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<SalesOrder> GetSalesOrders(string CompanyID, SalesOrderFilter Filter)
        {
            int intTotalCount = 0;
            return GetSalesOrders(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<SalesOrder> GetSalesOrders(string CompanyID, SalesOrderFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrders(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrder> GetSalesOrders(string CompanyID, SalesOrderFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrder> objReturn = null;
            SalesOrder objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrder>();

                strSQL = "SELECT s.* " +
                         "FROM SalesOrder (NOLOCK) s " +
                         "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.PONumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PONumber, "s.PONumber");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrder), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrder(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }

                TotalRecord = objReturn.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
                objNew = null;
            }
            return objReturn;
        }
    }
}
