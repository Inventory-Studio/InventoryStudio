using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;

namespace ISLibrary
{
    public class Inventory : BaseClass
    {
        public string InventoryID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(InventoryID); } }
        public string CompanyID { get; set; }
        public string ParentInventoryID { get; set; }
        public string ItemID { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public int OnHand { get; set; }
        public int Available { get; set; }
        public string CartonNumber { get; set; }
        public string VendorCartonNumber { get; set; }
        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
        public string LocationID { get; set; }
        public string BinID { get; set; }
        public string ReceiptLineID { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private List<Inventory> mChildInventories = null;
        public List<Inventory> ChildInventories
        {
            get
            {
                InventoryFilter objFilter = null;

                try
                {
                    if (mChildInventories == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(InventoryID))
                    {
                        objFilter = new InventoryFilter();
                        objFilter.ParentInventoryID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ParentInventoryID.SearchString = InventoryID;
                        mChildInventories = Inventory.GetInventories(CompanyID, objFilter);
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
                return mChildInventories;
            }
            set
            {
                mChildInventories = value;
            }
        }

        public Inventory()
        {
        }

        public Inventory(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public Inventory(string CompanyID, string InventoryID)
        {
            this.CompanyID = CompanyID;
            this.InventoryID = InventoryID;
            Load();
        }

        public Inventory(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT i.*, it.ItemNumber, it.ItemName " +
                         "FROM Inventory i (NOLOCK) " +
                         "INNER JOIN Item it (NOLOCK) on it.ItemID = i.ItemID " + 
                         "WHERE InventoryID=" + Database.HandleQuote(InventoryID);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InventoryID=" + InventoryID + " is not found");
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
            base.Load();

            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("InventoryID")) InventoryID = Convert.ToString(objRow["InventoryID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ParentInventoryID")) ParentInventoryID = Convert.ToString(objRow["ParentInventoryID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("OnHand")) OnHand = Convert.ToInt32(objRow["OnHand"]);
                if (objColumns.Contains("Available")) Available = Convert.ToInt32(objRow["Available"]);
                if (objColumns.Contains("CartonNumber")) CartonNumber = Convert.ToString(objRow["CartonNumber"]);
                if (objColumns.Contains("VendorCartonNumber")) VendorCartonNumber = Convert.ToString(objRow["VendorCartonNumber"]);
                if (objColumns.Contains("SerialNumber")) SerialNumber = Convert.ToString(objRow["SerialNumber"]);
                if (objColumns.Contains("LotNumber")) LotNumber = Convert.ToString(objRow["LotNumber"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("BinID")) BinID = Convert.ToString(objRow["BinID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (objColumns.Contains("ItemNumber")) ItemNumber = Convert.ToString(objRow["ItemNumber"]);
                if (objColumns.Contains("ItemName")) ItemName = Convert.ToString(objRow["ItemName"]);

                if (string.IsNullOrEmpty(InventoryID)) throw new Exception("Missing InventoryID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
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

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy name must be entered");
                if (!IsNew) throw new Exception("Create cannot be performed, InventoryID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentInventoryID"] = ParentInventoryID;
                dicParam["ItemID"] = ItemID;
                dicParam["OnHand"] = OnHand;
                dicParam["Available"] = Available;
                dicParam["CartonNumber"] = CartonNumber;
                dicParam["VendorCartonNumber"] = VendorCartonNumber;
                dicParam["SerialNumber"] = SerialNumber;
                dicParam["LotNumber"] = LotNumber;
                dicParam["LocationID"] = LocationID;
                dicParam["BinID"] = BinID;
                dicParam["CreatedBy"] = CreatedBy;

                InventoryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Inventory"), objConn, objTran).ToString();

                //if(ChildInventories != null)
                //{
                //    foreach (Inventory objInventory in ChildInventories)
                //    {
                //        objInventory.ParentInventoryID = InventoryID;
                //        objInventory.CreatedBy = CreatedBy;
                //        objInventory.Create(objConn, objTran);

                //    }
                //}

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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy name must be entered");
                if (IsNew) throw new Exception("Update cannot be performed, InventoryID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");


                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentInventoryID"] = ParentInventoryID;
                dicParam["ItemID"] = ItemID;
                dicParam["OnHand"] = OnHand;
                dicParam["Available"] = Available;
                dicParam["CartonNumber"] = CartonNumber;
                dicParam["VendorCartonNumber"] = VendorCartonNumber;
                dicParam["SerialNumber"] = SerialNumber;
                dicParam["LotNumber"] = LotNumber;
                dicParam["LocationID"] = LocationID;
                dicParam["BinID"] = BinID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["InventoryID"] = InventoryID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Inventory"), objConn, objTran);

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
            return true;
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
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, InventoryID is missing");

                dicDParam["InventoryID"] = InventoryID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Inventory"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM Inventory (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID);
            if (!string.IsNullOrEmpty(ItemID)) strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID);
            if (!string.IsNullOrEmpty(BinID)) strSQL += "AND p.BinID=" + Database.HandleQuote(BinID);
            if (!string.IsNullOrEmpty(CartonNumber)) strSQL += "AND p.CartonNumber=" + Database.HandleQuote(CartonNumber);
            if (!string.IsNullOrEmpty(LotNumber)) strSQL += "AND p.LotNumber=" + Database.HandleQuote(LotNumber);
            if (!string.IsNullOrEmpty(ParentInventoryID)) strSQL += "AND p.ParentInventoryID=" + Database.HandleQuote(ParentInventoryID);

            if (!string.IsNullOrEmpty(InventoryID)) strSQL += "AND p.InventoryID<>" + Database.HandleQuote(InventoryID);
            return Database.HasRows(strSQL);
        }

        public static Inventory GetInventory(string CompanyID, InventoryFilter Filter)
        {
            List<Inventory> objInventories = null;
            Inventory objReturn = null;

            try
            {
                objInventories = GetInventories(CompanyID, Filter);
                if (objInventories != null && objInventories.Count >= 1) objReturn = objInventories[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInventories = null;
            }
            return objReturn;
        }

        public static List<Inventory> GetInventories(string CompanyID)
        {
            int intTotalCount = 0;
            return GetInventories(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Inventory> GetInventories(string CompanyID, InventoryFilter Filter)
        {
            int intTotalCount = 0;
            return GetInventories(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Inventory> GetInventories(string CompanyID, InventoryFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInventories(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Inventory> GetInventories(string CompanyID, InventoryFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Inventory> objReturn = null;
            Inventory objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<Inventory>();

                    strSQL = "SELECT i.*, it.ItemNumber, it.ItemName " +
                             "FROM Inventory i (NOLOCK) " +
                             "INNER JOIN Item it (NOLOCK) on it.ItemID = i.ItemID " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.InventoryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InventoryID, "i.InventoryID");
                        if (Filter.CompanyID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "i.CompanyID");
                        if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "i.ItemID");
                        if (Filter.BinID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BinID, "i.BinID");
                        if (Filter.ParentInventoryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentInventoryID, "i.ParentInventoryID");
                        if (Filter.CartonNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CartonNumber, "i.CartonNumber");
                        if (Filter.LotNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LotNumber, "i.LotNumber");
                    }

                    if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeName" : Utility.CustomSorting.GetSortExpression(typeof(Inventory), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new Inventory(objData.Tables[0].Rows[i]);
                            objNew.IsLoaded = true;
                            objReturn.Add(objNew);
                        }
                    }
                }
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
