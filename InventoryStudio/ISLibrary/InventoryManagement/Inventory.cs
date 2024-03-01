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
    public class Inventory : BaseClass
    {
        public string InventoryID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(InventoryID); }
        }
        public string CompanyID { get; set; }
        public string? ParentInventoryID { get; set; }
        public string ItemID { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public decimal OnHand { get; set; }
        public decimal Available { get; set; }
        public decimal Committed { get; set; }
        public string CartonNumber { get; set; }
        public string VendorCartonNumber { get; set; }
        public string InventoryNumber { get; set; }
        public DateTime? LotNumberDate { get; set; }
        public string LocationID { get; set; }
        public string LocationName { get; set; }
        public string LocationNumber { get; set; }
        public string BinID { get; set; }
        public string BinNumber { get; set; }
        public string Label { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        public Inventory()
        {
        }

        public Inventory( string Id)
        {
            this.InventoryID = Id;
            this.Load();
        }

        public Inventory(DataRow objRow)
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
                         "FROM Inventory (NOLOCK) " +
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
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("InventoryID")) InventoryID = Convert.ToString(objRow["InventoryID"]);
                //if (objColumns.Contains("LocationName")) LocationName = Convert.ToString(objRow["LocationName"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InventoryID)) throw new Exception("Missing AdjustmentID in the datarow");
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

                InventoryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Inventory"), objConn, objTran)
                    .ToString();


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

            LogAuditData(enumActionType.Update);
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM Inventory (NOLOCK) p " +
                     "WHERE (p.BinID=" + Database.HandleQuote(BinID);

            if (!string.IsNullOrEmpty(CartonNumber))
            {
                strSQL += "AND p.CartonNumber<>" + Database.HandleQuote(CartonNumber);
            }
            if (!string.IsNullOrEmpty(VendorCartonNumber))
            {
                strSQL += "AND p.VendorCartonNumber<>" + Database.HandleQuote(VendorCartonNumber);
            }
            if (!string.IsNullOrEmpty(InventoryNumber))
            {
                strSQL += "AND p.InventoryNumber<>" + Database.HandleQuote(InventoryNumber);
            }
            if (!string.IsNullOrEmpty(ParentInventoryID))
            {
                strSQL += "AND p.ParentInventoryID<>" + Database.HandleQuote(ParentInventoryID);
            }
            strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID) + ")";
            if (!string.IsNullOrEmpty(InventoryID))
            {
                strSQL += "AND p.InventoryID<>" + Database.HandleQuote(InventoryID);
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
                     "FROM Inventory (NOLOCK) p " +
                     "WHERE (p.BinID=" + Database.HandleQuote(BinID);
                
                if (!string.IsNullOrEmpty(CartonNumber))
                {
                    strSQL += "AND p.CartonNumber<>" + Database.HandleQuote(CartonNumber);
                }
                if (!string.IsNullOrEmpty(VendorCartonNumber))
                {
                    strSQL += "AND p.VendorCartonNumber<>" + Database.HandleQuote(VendorCartonNumber);
                }
                if (!string.IsNullOrEmpty(InventoryNumber))
                {
                    strSQL += "AND p.InventoryNumber<>" + Database.HandleQuote(InventoryNumber);
                }
                if (!string.IsNullOrEmpty(ParentInventoryID))
                {
                    strSQL += "AND p.ParentInventoryID<>" + Database.HandleQuote(ParentInventoryID);
                }
                strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID) + ")";

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

        //public static Adjustment GetAdjustment(string CompanyID, AdjustmentFilter Filter)
        //{
        //    List<Adjustment> objAdjustments = null;
        //    Adjustment objReturn = null;

        //    try
        //    {
        //        objAdjustments = GetAdjustments(CompanyID, Filter);
        //        if (objAdjustments != null && objAdjustments.Count >= 1) objReturn = objAdjustments[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objAdjustments = null;
        //    }

        //    return objReturn;
        //}

        //public static List<Adjustment> GetAdjustments(string CompanyID)
        //{
        //    int intTotalCount = 0;
        //    return GetAdjustments(CompanyID, null, null, null, out intTotalCount);
        //}

        //public static List<Adjustment> GetAdjustments(string CompanyID, AdjustmentFilter Filter)
        //{
        //    int intTotalCount = 0;
        //    return GetAdjustments(CompanyID, Filter, null, null, out intTotalCount);
        //}

        //public static List<Adjustment> GetAdjustments(string CompanyID, AdjustmentFilter Filter, int? PageSize,
        //    int? PageNumber, out int TotalRecord)
        //{
        //    return GetAdjustments(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        //}

        //public static List<Adjustment> GetAdjustments(string CompanyID, AdjustmentFilter Filter,
        //    string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        //{
        //    List<Adjustment> objReturn = null;
        //    Adjustment objNew = null;
        //    DataSet objData = null;
        //    string strSQL = string.Empty;

        //    try
        //    {
        //        TotalRecord = 0;

        //        objReturn = new List<Adjustment>();

        //        strSQL = "SELECT * " +
        //                 "FROM Adjustment (NOLOCK) " +
        //                 "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
        //        if (Filter != null)
        //        {
        //            if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "LocationID");
        //        }

        //        if (PageSize != null && PageNumber != null)
        //            strSQL = Database.GetPagingSQL(strSQL,
        //                string.IsNullOrEmpty(SortExpression)
        //                    ? "AdjustmentID"
        //                    : Utility.CustomSorting.GetSortExpression(typeof(Adjustment), SortExpression),
        //                string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
        //        objData = Database.GetDataSet(strSQL);

        //        if (objData != null && objData.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
        //            {
        //                objNew = new Adjustment(objData.Tables[0].Rows[i]);
        //                objNew.IsLoaded = true;
        //                objReturn.Add(objNew);
        //            }
        //        }

        //        TotalRecord = objReturn.Count();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objData = null;
        //    }

        //    return objReturn;
        //}


    }
}