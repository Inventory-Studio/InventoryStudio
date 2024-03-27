using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using ISLibrary.OrderManagement;
using System.Data.Common;
using System.Transactions;
using System.ComponentModel;

namespace ISLibrary
{
    public class Fulfillment : BaseClass
    {
        public string FulfillmentID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(FulfillmentID); }
        } 
        

        public string CompanyID { get; set; }
        public string SalesOrderID { get; set; }
        public DateTime? TranDate { get; set; }
        public string LocationID { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsPickedUp { get; set; }
        public bool IsPicked { get; set; }
        public bool IsPacked { get; set; }
        public bool IsShipped { get; set; }
        public string PrintedBy { get; set; }
        public string PickedUpBy { get; set; }
        public string PickedBy { get; set; }
        public string PackedBy { get; set; }
        public string ShippedBy { get; set; }
        public DateTime? PrintedOn { get; set; }
        public DateTime? PickedUpOn { get; set; }
        public DateTime? PickedOn { get; set; }
        public DateTime? PackedOn { get; set; }
        public DateTime? ShippedOn { get; set; }
        public string PackingSlipPath { get; set; }
        public string FulfillmentBatchID { get; set; }
        public string BatchPrintNumber { get; set; }
        public string FulfillmentProblemID { get; set; }
        public string FulfillmentGroupID { get; set; }
        public enum enumFulfillmentStatus
        {
            [Description("Pending")]
            Pending,
            [Description("Picked Up")]
            PickedUp,
            [Description("Picked")]
            Picked,
            [Description("Shipped")]
            Shipped
        }
        public string Status { get; set; }
        public bool IsInactive { get; set; }
        public string PresetPackageDimensionID { get; set; }
        public decimal? PresetPackageWeight { get; set; }
        public string PresetPackageWeightUnit { get; set; }
        public string PresetPackageShippingMethod { get; set; }
        public bool BypassSourceAddressValidation { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        private List<FulfillmentLine> mFulfillmentLine = null;
        public List<FulfillmentLine> FulfillmentLines
        {
            get
            {
                FulfillmentLineFilter objFilter = null;

                try
                {
                    if (mFulfillmentLine == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(FulfillmentID))
                    {
                        objFilter = new FulfillmentLineFilter();
                        objFilter.FulfillmentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.FulfillmentID.SearchString = FulfillmentID;
                        mFulfillmentLine = FulfillmentLine.GetFulfillmentLines(CompanyID, objFilter);
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
                return mFulfillmentLine;
            }
            set
            {
                mFulfillmentLine = value;
            }
        }

        private SalesOrder mSalesOrder = null;
        public SalesOrder SalesOrder
        {
            get
            {
                SalesOrderFilter objFilter = null;

                try
                {
                    if (mSalesOrder == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(SalesOrderID))
                    {

                        mSalesOrder = new SalesOrder(CompanyID, SalesOrderID);
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
                return mSalesOrder;
            }
            set
            {
                mSalesOrder = value;
            }
        }

        public List<SalesOrderLine> SalesOrderLines { get; set; }
        public Fulfillment()
        {
        }

        public Fulfillment( string CompanyID, string FulfillmentID)
        {
            this.CompanyID = CompanyID;
            this.FulfillmentID = FulfillmentID;
            this.Load();
        }

        public Fulfillment(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM Fulfillment (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                        " AND FulfillmentID=" + Database.HandleQuote(FulfillmentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentID=" + FulfillmentID + " is not found");
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

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("FulfillmentID")) FulfillmentID = Convert.ToString(objRow["FulfillmentID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("TranDate")) TranDate = Convert.ToDateTime(objRow["TranDate"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("IsPrinted")) IsPrinted = Convert.ToBoolean(objRow["IsPrinted"]);
                if (objColumns.Contains("IsPickedUp")) IsPickedUp = Convert.ToBoolean(objRow["IsPickedUp"]);
                if (objColumns.Contains("IsPicked")) IsPicked = Convert.ToBoolean(objRow["IsPicked"]);
                if (objColumns.Contains("IsPacked")) IsPacked = Convert.ToBoolean(objRow["IsPacked"]);
                if (objColumns.Contains("IsShipped")) IsShipped = Convert.ToBoolean(objRow["IsShipped"]);
                if (objColumns.Contains("PrintedBy")) PrintedBy = Convert.ToString(objRow["PrintedBy"]);
                if (objColumns.Contains("PickedUpBy")) PickedUpBy = Convert.ToString(objRow["PickedUpBy"]);
                if (objColumns.Contains("PickedBy")) PickedBy = Convert.ToString(objRow["PickedBy"]);
                if (objColumns.Contains("PackedBy")) PackedBy = Convert.ToString(objRow["PackedBy"]);
                if (objColumns.Contains("ShippedBy")) ShippedBy = Convert.ToString(objRow["ShippedBy"]);
                if (objColumns.Contains("PrintedOn") && objRow["PrintedOn"] != DBNull.Value) PrintedOn = Convert.ToDateTime(objRow["PrintedOn"]);
                if (objColumns.Contains("PickedUpOn") && objRow["PickedUpOn"] != DBNull.Value) PickedUpOn = Convert.ToDateTime(objRow["PickedUpOn"]);
                if (objColumns.Contains("PickedOn") && objRow["PickedOn"] != DBNull.Value) PickedOn = Convert.ToDateTime(objRow["PickedOn"]);
                if (objColumns.Contains("PackedOn") && objRow["PackedOn"] != DBNull.Value) PackedOn = Convert.ToDateTime(objRow["PackedOn"]);
                if (objColumns.Contains("ShippedOn") && objRow["ShippedOn"] != DBNull.Value) ShippedOn = Convert.ToDateTime(objRow["ShippedOn"]);
                if (objColumns.Contains("PackingSlipPath")) PackingSlipPath = Convert.ToString(objRow["PackingSlipPath"]);
                if (objColumns.Contains("FulfillmentBatchID")) FulfillmentBatchID = Convert.ToString(objRow["FulfillmentBatchID"]);
                if (objColumns.Contains("BatchPrintNumber")) BatchPrintNumber = Convert.ToString(objRow["BatchPrintNumber"]);
                if (objColumns.Contains("FulfillmentProblemID")) FulfillmentProblemID = Convert.ToString(objRow["FulfillmentProblemID"]);
                if (objColumns.Contains("FulfillmentGroupID")) FulfillmentGroupID = Convert.ToString(objRow["FulfillmentGroupID"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("IsInactive")) IsInactive = Convert.ToBoolean(objRow["IsInactive"]);
                if (objColumns.Contains("PresetPackageDimensionID")) PresetPackageDimensionID = Convert.ToString(objRow["PresetPackageDimensionID"]);
                if (objColumns.Contains("PresetPackageWeight") && objRow["PresetPackageWeight"] != DBNull.Value) PresetPackageWeight = Convert.ToDecimal(objRow["PresetPackageWeight"]);
                if (objColumns.Contains("PresetPackageWeightUnit")) PresetPackageWeightUnit = Convert.ToString(objRow["PresetPackageWeightUnit"]);
                if (objColumns.Contains("PresetPackageShippingMethod")) PresetPackageShippingMethod = Convert.ToString(objRow["PresetPackageShippingMethod"]);
                if (objColumns.Contains("BypassSourceAddressValidation")) BypassSourceAddressValidation = Convert.ToBoolean(objRow["BypassSourceAddressValidation"]);


                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
           

                if (string.IsNullOrEmpty(FulfillmentID)) throw new Exception("Missing FulfillmentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, AdjustmentID already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["TranDate"] = DateTime.UtcNow; 
                dicParam["LocationID"] = LocationID;
                dicParam["IsPrinted"] = IsPrinted;
                dicParam["IsPickedUp"] = IsPickedUp;
                dicParam["IsPicked"] = IsPicked;
                dicParam["IsPacked"] = IsPacked;
                dicParam["IsShipped"] = IsShipped;
                dicParam["PrintedBy"] = PrintedBy;
                dicParam["PickedUpBy"] = PickedUpBy;
                dicParam["PickedBy"] = PickedBy;
                dicParam["PackedBy"] = PackedBy;
                dicParam["ShippedBy"] = ShippedBy;
                dicParam["PrintedOn"] = PrintedOn ;
                dicParam["PickedUpOn"] = PickedUpOn ;
                dicParam["PickedOn"] = PickedOn ;
                dicParam["PackedOn"] = PackedOn ;
                dicParam["ShippedOn"] = ShippedOn ;
                dicParam["PackingSlipPath"] = PackingSlipPath;
                dicParam["FulfillmentBatchID"] = FulfillmentBatchID;
                dicParam["BatchPrintNumber"] = BatchPrintNumber;
                dicParam["FulfillmentProblemID"] = FulfillmentProblemID;
                dicParam["FulfillmentGroupID"] = FulfillmentGroupID;
                dicParam["Status"] = enumFulfillmentStatus.Pending;
                dicParam["IsInactive"] = IsInactive;
                dicParam["PresetPackageDimensionID"] = PresetPackageDimensionID;
                dicParam["PresetPackageWeight"] = PresetPackageWeight;
                dicParam["PresetPackageWeightUnit"] = PresetPackageWeightUnit;
                dicParam["PresetPackageShippingMethod"] = PresetPackageShippingMethod;
                dicParam["BypassSourceAddressValidation"] = BypassSourceAddressValidation;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                FulfillmentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Fulfillment"), objConn, objTran)
                    .ToString();

               
             

                if (SalesOrderLines != null)
                {
                    foreach (SalesOrderLine objSalesOrderLine in SalesOrderLines)
                    {
                        SalesOrderLine objExistSalesOrderLine = new SalesOrderLine(CompanyID, objSalesOrderLine.SalesOrderLineID);
                        
                        if (objExistSalesOrderLine.Quantity > objExistSalesOrderLine.QuantityFulfilled)
                        {
                            decimal decQuantity = objSalesOrderLine.Quantity;


                            decimal decQtyPerUnit = 1;
                            if (objSalesOrderLine.ItemUnit != null)
                            {
                                decQtyPerUnit = objSalesOrderLine.ItemUnit.Quantity;
                            }

                            FulfillmentLine objFulfillmentLine = new FulfillmentLine();
                            objFulfillmentLine.CompanyID = CompanyID;
                            objFulfillmentLine.LocationID = LocationID;
                            objFulfillmentLine.FulfillmentID = FulfillmentID;
                            objFulfillmentLine.SalesOrderLineID = objSalesOrderLine.SalesOrderLineID;
                            objFulfillmentLine.CreatedBy = CreatedBy;
                            objFulfillmentLine.ParentKey = FulfillmentID;
                            objFulfillmentLine.ParentObject = "Fulfillment";

                            if (objSalesOrderLine.Item.ItemType == Item.enumItemType.Kit)
                            {
                                //Get Item Kit

                                foreach (ItemKit objItemKit in objSalesOrderLine.Item.ItemKits)
                                {
                                    objFulfillmentLine.ItemID = objItemKit.ChildItemID;
                                    objFulfillmentLine.Quantity = decQuantity * objItemKit.Quantity;
                                    objFulfillmentLine.ItemUnitID = objItemKit.ItemUnitID;
                                    objFulfillmentLine.Create(objConn, objTran);
                                }
                            }
                            else
                            {
                                objFulfillmentLine.ItemID = objSalesOrderLine.ItemID;
                                objFulfillmentLine.Quantity = decQuantity * decQtyPerUnit;
                                objFulfillmentLine.ItemUnitID = objSalesOrderLine.ItemUnitID;
                                objFulfillmentLine.Create(objConn, objTran);
                            }


                            foreach(SalesOrderLineDetail objSalesOrderLineDetail in objSalesOrderLine.SalesOrderLineDetails)
                            {
                                FulfillmentLineDetail objFulfillmentLineDetail = new FulfillmentLineDetail();
                                objFulfillmentLineDetail.CompanyID = CompanyID;
                                objFulfillmentLineDetail.FulfillmentLineID = objFulfillmentLine.FulfillmentLineID;
                                objFulfillmentLineDetail.BinID = objSalesOrderLineDetail.BinID;
                                objFulfillmentLineDetail.Quantity = objSalesOrderLineDetail.Quantity;
                                objFulfillmentLineDetail.InventoryNumber = objSalesOrderLineDetail.InventoryNumber;
                                objFulfillmentLineDetail.InventoryDetailID = objSalesOrderLineDetail.InventoryDetailID;
                                objFulfillmentLineDetail.CreatedBy = CreatedBy;
                                objFulfillmentLineDetail.ParentKey = objFulfillmentLine.FulfillmentLineID;
                                objFulfillmentLineDetail.ParentObject = "FulfillmentLine";
                                objFulfillmentLineDetail.Create(objConn, objTran);
                            }



                            objExistSalesOrderLine.QuantityFulfilled += objSalesOrderLine.Quantity;
                            objExistSalesOrderLine.UpdatedBy = CreatedBy;
                            objExistSalesOrderLine.ParentKey = FulfillmentID;
                            objExistSalesOrderLine.ParentObject = "Fulfillment";
                            objExistSalesOrderLine.SalesOrderLineDetails = null;
                            objExistSalesOrderLine.Update(objConn, objTran);

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
            ItemParent objItemParent = null;
            try
            {
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ID is missing");

   

                dicParam["LocationID"] = LocationID;
                dicParam["IsPrinted"] = IsPrinted;
                dicParam["IsPickedUp"] = IsPickedUp;
                dicParam["IsPicked"] = IsPicked;
                dicParam["IsPacked"] = IsPacked;
                dicParam["IsShipped"] = IsShipped;
                dicParam["PrintedBy"] = PrintedBy;
                dicParam["PickedUpBy"] = PickedUpBy;
                dicParam["PickedBy"] = PickedBy;
                dicParam["PackedBy"] = PackedBy;
                dicParam["ShippedBy"] = ShippedBy;
                dicParam["PrintedOn"] = PrintedOn;
                dicParam["PickedUpOn"] = PickedUpOn;
                dicParam["PickedOn"] = PickedOn;
                dicParam["PackedOn"] = PackedOn;
                dicParam["ShippedOn"] = ShippedOn;
                dicParam["PackingSlipPath"] = PackingSlipPath;
                dicParam["FulfillmentBatchID"] = FulfillmentBatchID;
                dicParam["BatchPrintNumber"] = BatchPrintNumber;
                dicParam["FulfillmentProblemID"] = FulfillmentProblemID;
                dicParam["FulfillmentGroupID"] = FulfillmentGroupID;
                dicParam["Status"] = Status;
                dicParam["IsInactive"] = IsInactive;
                dicParam["PresetPackageDimensionID"] = PresetPackageDimensionID;
                dicParam["PresetPackageWeight"] = PresetPackageWeight;
                dicParam["PresetPackageWeightUnit"] = PresetPackageWeightUnit;
                dicParam["PresetPackageShippingMethod"] = PresetPackageShippingMethod;
                dicParam["BypassSourceAddressValidation"] = BypassSourceAddressValidation;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["UpdatedBy"] = UpdatedBy;

                dicWParam["FulfillmentID"] = FulfillmentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Fulfillment"), objConn, objTran);

              

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

        public static Fulfillment GetFulfillment(string CompanyID, FulfillmentFilter Filter)
        {
            List<Fulfillment> objAdjustments = null;
            Fulfillment objReturn = null;

            try
            {
                objAdjustments = GetFulfillments(CompanyID, Filter);
                if (objAdjustments != null && objAdjustments.Count >= 1) objReturn = objAdjustments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAdjustments = null;
            }

            return objReturn;
        }

        public static List<Fulfillment> GetFulfillments(string CompanyID)
        {
            int intTotalCount = 0;
            return GetFulfillments(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Fulfillment> GetFulfillments(string CompanyID, FulfillmentFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillments(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Fulfillment> GetFulfillments(string CompanyID, FulfillmentFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetFulfillments(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Fulfillment> GetFulfillments(string CompanyID, FulfillmentFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Fulfillment> objReturn = null;
            Fulfillment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Fulfillment>();

                strSQL = "SELECT * " +
                         "FROM Fulfillment (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.Status != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Status, "Status");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "FulfillmentID"
                            : Utility.CustomSorting.GetSortExpression(typeof(Fulfillment), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Fulfillment(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }

                TotalRecord = objReturn.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }

            return objReturn;
        }


    }
}