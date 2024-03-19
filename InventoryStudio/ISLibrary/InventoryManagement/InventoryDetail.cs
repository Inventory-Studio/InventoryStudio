using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using System.Security;
using System.ComponentModel.Design;

namespace ISLibrary
{
    public class InventoryDetail : BaseClass
    {
        public string InventoryDetailID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(InventoryDetailID); }
        }
        public string CompanyID { get; set; }
        public string? ParentInventoryID { get; set; }
        public string ItemID { get; set; }
        public string ItemConfigID { get; set; }
        public string ItemInventoryStatusID { get; set; }
        public decimal OnHand { get; set; }
        public decimal Available { get; set; }
        public decimal ChangedQty { get; set; }
        //public decimal Committed { get; set; }
        public string CartonNumber { get; set; }
        public string VendorCartonNumber { get; set; }
        public string InventoryNumber { get; set; }
        public DateTime? LotNumberDate { get; set; }
        public string LocationID { get; set; }
        public string BinID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        private Inventory mInventory = null;
        public Inventory Inventory
        {
            get
            {
                InventoryFilter objFilter = null;
                try
                {
                    if (mInventory == null && !string.IsNullOrEmpty(LocationID) && !string.IsNullOrEmpty(ItemID))
                    {
                        objFilter = new InventoryFilter();
                        objFilter.LocationID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.LocationID.SearchString = LocationID;
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mInventory = Inventory.GetInventory(objFilter);
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
                return mInventory;
            }
            set
            {
                mInventory = value;
            }
        }

        private Bin mBin = null;
        public Bin Bin
        {
            get
            {
                BinFilter objFilter = null;
                try
                {
                    if (mBin == null && !string.IsNullOrEmpty(BinID) && !string.IsNullOrEmpty(CompanyID))
                    {
                        objFilter = new BinFilter();
                        objFilter.BinID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BinID.SearchString = BinID;
                        mBin = Bin.GetBin(CompanyID,objFilter);
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
                return mBin;
            }
            set
            {
                mBin = value;
            }
        }
        
        private Location mLocation = null;
        public Location Location
        {
            get
            {
                LocationFilter objFilter = null;
                try
                {
                    if (mLocation == null && !string.IsNullOrEmpty(LocationID) && !string.IsNullOrEmpty(CompanyID))
                    {
                        objFilter = new LocationFilter();
                        objFilter.LocationID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.LocationID.SearchString = LocationID;
                        mLocation = Location.GetLocation(CompanyID, objFilter);
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
                return mLocation;
            }
            set
            {
                mLocation = value;
            }
        }
        public InventoryDetail()
        {
        }

        public InventoryDetail( string Id)
        {
            this.InventoryDetailID = Id;
            this.Load();
        }

        public InventoryDetail(DataRow objRow)
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
                         "FROM InventoryDetail (NOLOCK) " +
                         "WHERE InventoryDetailID=" + Database.HandleQuote(InventoryDetailID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InventoryDetailID=" + InventoryDetailID + " is not found");
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

                if (objColumns.Contains("InventoryDetailID")) InventoryDetailID = Convert.ToString(objRow["InventoryDetailID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ParentInventoryID")) ParentInventoryID = Convert.ToString(objRow["ParentInventoryID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ItemConfigID")) ItemConfigID = Convert.ToString(objRow["ItemConfigID"]);
                if (objColumns.Contains("ItemInventoryStatusID")) ItemInventoryStatusID = Convert.ToString(objRow["ItemInventoryStatusID"]);
                if (objColumns.Contains("OnHand")) OnHand = Convert.ToDecimal(objRow["OnHand"]);
                if (objColumns.Contains("Available")) Available = Convert.ToDecimal(objRow["Available"]);
                if (objColumns.Contains("CartonNumber")) CartonNumber = Convert.ToString(objRow["CartonNumber"]);
                if (objColumns.Contains("VendorCartonNumber")) VendorCartonNumber = Convert.ToString(objRow["VendorCartonNumber"]);
                if (objColumns.Contains("InventoryNumber")) InventoryNumber = Convert.ToString(objRow["InventoryNumber"]);
                if (objColumns.Contains("LotNumberDate") && objRow["LotNumberDate"] != DBNull.Value) LotNumberDate = Convert.ToDateTime(objRow["LotNumberDate"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("BinID")) BinID = Convert.ToString(objRow["BinID"]);
                //if (objColumns.Contains("LocationName")) LocationName = Convert.ToString(objRow["LocationName"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InventoryDetailID)) throw new Exception("Missing AdjustmentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, InventoryID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentInventoryID"] = ParentInventoryID;
                dicParam["ItemID"] = ItemID;
                //dicParam["ItemConfigID"] = ItemConfigID;
                //dicParam["ItemInventoryStatusID"] = ItemInventoryStatusID;
                dicParam["OnHand"] = OnHand;
                dicParam["Available"] = Available;
                dicParam["CartonNumber"] = CartonNumber;
                dicParam["VendorCartonNumber"] = VendorCartonNumber;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["LotNumberDate"] = LotNumberDate;
                dicParam["LocationID"] = LocationID;
                dicParam["BinID"] = BinID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                InventoryDetailID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InventoryDetail"), objConn, objTran)
                    .ToString();

                if (Inventory != null) {
                    Inventory.QtyOnHand += OnHand;
                    Inventory.QtyAvailable += Available;
                    Inventory.Update();
                }
                else
                {
                    Inventory objInventory = new Inventory();
                    objInventory.ItemID = ItemID;
                    objInventory.LocationID = LocationID;
                    objInventory.QtyOnHand = OnHand;
                    objInventory.QtyAvailable = Available;
                    objInventory.Create();
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
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
               

                dicParam["ParentInventoryID"] = ParentInventoryID;
                //dicParam["ItemConfigID"] = ItemConfigID;
                //dicParam["ItemInventoryStatusID"] = ItemInventoryStatusID;
                dicParam["OnHand"] = OnHand;
                dicParam["Available"] = Available;
                dicParam["CartonNumber"] = CartonNumber;
                dicParam["VendorCartonNumber"] = VendorCartonNumber;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["LotNumberDate"] = LotNumberDate;
                dicParam["LocationID"] = LocationID;
                dicParam["BinID"] = BinID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["InventoryDetailID"] = InventoryDetailID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "InventoryDetail"), objConn, objTran);

                if (Inventory != null)
                {//ChangedQty used for adjustmemt and transfer
                    Inventory.QtyOnHand += ChangedQty;
                    Inventory.QtyAvailable += ChangedQty;
                    Inventory.Update();
                }
                else
                {
                    Inventory objInventory = new Inventory();
                    objInventory.ItemID = ItemID;
                    objInventory.LocationID = LocationID;
                    objInventory.QtyOnHand = ChangedQty;
                    objInventory.QtyAvailable = ChangedQty;
                    objInventory.Create();
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM InventoryDetail (NOLOCK) p " +
                     "WHERE (p.BinID=" + Database.HandleQuote(BinID);

            if (!string.IsNullOrEmpty(CartonNumber))
            {
                strSQL += "AND p.CartonNumber=" + Database.HandleQuote(CartonNumber);
            }
            if (!string.IsNullOrEmpty(VendorCartonNumber))
            {
                strSQL += "AND p.VendorCartonNumber=" + Database.HandleQuote(VendorCartonNumber);
            }
            if (!string.IsNullOrEmpty(InventoryNumber))
            {
                strSQL += "AND p.InventoryNumber=" + Database.HandleQuote(InventoryNumber);
            }
            if (!string.IsNullOrEmpty(ParentInventoryID))
            {
                strSQL += "AND p.ParentInventoryID=" + Database.HandleQuote(ParentInventoryID);
            }
            strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID) + ")";
            if (!string.IsNullOrEmpty(InventoryDetailID))
            {
                strSQL += "AND p.InventoryID=" + Database.HandleQuote(InventoryDetailID);
            }           

            return Database.HasRows(strSQL);
        }

        public bool GetUnqiueInventory()
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            bool result = false;
            try
            {
                strSQL = "SELECT TOP 1 p.* " +
                     "FROM InventoryDetail (NOLOCK) p " +
                     "WHERE (p.BinID=" + Database.HandleQuote(BinID);
                
                if (!string.IsNullOrEmpty(CartonNumber))
                {
                    strSQL += "AND p.CartonNumber=" + Database.HandleQuote(CartonNumber);
                }
                if (!string.IsNullOrEmpty(VendorCartonNumber))
                {
                    strSQL += "AND p.VendorCartonNumber=" + Database.HandleQuote(VendorCartonNumber);
                }
                if (!string.IsNullOrEmpty(InventoryNumber))
                {
                    strSQL += "AND p.InventoryNumber=" + Database.HandleQuote(InventoryNumber);
                }
                if (!string.IsNullOrEmpty(ParentInventoryID))
                {
                    strSQL += "AND p.ParentInventoryID=" + Database.HandleQuote(ParentInventoryID);
                }
                strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID);
                strSQL += "AND p.LocationID=" + Database.HandleQuote(LocationID) + ")";

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                    result = true;
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

            return result;
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
                if (string.IsNullOrEmpty(InventoryDetailID)) throw new Exception("Delete cannot be performed, CustomerID is missing");
                dicDParam["InventoryDetailID"] = InventoryDetailID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "InventoryDetail"), objConn, objTran);
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

        public static InventoryDetail GetInventoryDetail(string CompanyID, InventoryDetailFilter Filter)
        {
            List<InventoryDetail> objAdjustments = null;
            InventoryDetail objReturn = null;

            try
            {
                objAdjustments = GetInventoryDetails(CompanyID, Filter);
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

        public static List<InventoryDetail> GetInventoryDetails(string CompanyID)
        {
            int intTotalCount = 0;
            return GetInventoryDetails(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<InventoryDetail> GetInventoryDetails(string CompanyID, InventoryDetailFilter Filter)
        {
            int intTotalCount = 0;
            return GetInventoryDetails(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<InventoryDetail> GetInventoryDetails(string CompanyID, InventoryDetailFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetInventoryDetails(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<InventoryDetail> GetInventoryDetails(string CompanyID, InventoryDetailFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<InventoryDetail> objReturn = null;
            InventoryDetail objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<InventoryDetail>();

                strSQL = "SELECT * " +
                         "FROM InventoryDetail (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "LocationID");
                    if (Filter.BinID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BinID, "BinID");
                    if (Filter.InventoryNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InventoryNumber, "InventoryNumber");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "InventoryDetailID"
                            : Utility.CustomSorting.GetSortExpression(typeof(InventoryDetail), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new InventoryDetail(objData.Tables[0].Rows[i]);
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