using CLRFramework;
using ISLibrary.OrderManagement;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class Bin : BaseClass
    {
        public string BinID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(BinID); } }

        public string CompanyID { get; set; }

        public string LocationID { get; set; }

        public string BinNumber { get; set; } = null!;

        public bool IsLocked { get; set; }

        public bool AllowNegativeInventory { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        public Bin()
        {

        }

        public Bin(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public Bin(string CompanyID, string BinID)
        {
            this.CompanyID = CompanyID;
            this.BinID = BinID;
            Load();
        }

        public Bin(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("BinID")) BinID = Convert.ToString(objRow["BinID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("BinNumber")) BinNumber = Convert.ToString(objRow["BinNumber"]);
                if (objColumns.Contains("IsLocked")) IsLocked = Convert.ToBoolean(objRow["IsLocked"]);
                if (objColumns.Contains("AllowNegativeInventory")) AllowNegativeInventory = Convert.ToBoolean(objRow["AllowNegativeInventory"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
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
                strSQL = "SELECT b.* " +
                         "FROM Bin b (NOLOCK) " +
                         "WHERE b.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND b.BinID = " + Database.HandleQuote(BinID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Bin is not found");
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

        public bool Create()
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

        public bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();
            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(LocationID)) throw new Exception("LocationID is required");
                if (string.IsNullOrEmpty(BinNumber)) throw new Exception("BinNumber is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, BinID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["LocationID"] = LocationID;
                dicParam["BinNumber"] = BinNumber;
                dicParam["IsLocked"] = IsLocked;
                dicParam["AllowNegativeInventory"] = AllowNegativeInventory;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;
                BinID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Bin"), objConn, objTran).ToString();

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

        public bool Update()
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

        public bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(LocationID)) throw new Exception("LocationID is required");
                if (string.IsNullOrEmpty(BinNumber)) throw new Exception("BinNumber is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Create cannot be performed, BinID already exists");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["LocationID"] = LocationID;
                dicParam["BinNumber"] = BinNumber;
                dicParam["IsLocked"] = IsLocked;
                dicParam["AllowNegativeInventory"] = AllowNegativeInventory;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["BinID"] = BinID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Bin"), objConn, objTran);
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
                if (string.IsNullOrEmpty(BinID)) throw new Exception("Delete cannot be performed, CustomerID is missing");
                dicDParam["BinID"] = BinID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Bin"), objConn, objTran);
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
            strSQL = "SELECT TOP 1 b.* " +
                     "FROM Bin (NOLOCK) b " +
                     "WHERE b.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND b.BinNumber=" + Database.HandleQuote(BinNumber);
            return Database.HasRows(strSQL);
        }

        public static List<Bin> GetBins(string CompanyID)
        {
            int intTotalCount = 0;
            return GetBins(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Bin> GetBins(string CompanyID, BinFilter Filter)
        {
            int intTotalCount = 0;
            return GetBins(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Bin> GetBins(string CompanyID, BinFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetBins(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Bin> GetBins(string CompanyID, BinFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Bin> objReturn = null;
            Bin objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                TotalRecord = 0;
                objReturn = new List<Bin>();
                strSQL = "SELECT b.* " +
                                         "FROM Bin (NOLOCK) b " +
                                         "WHERE b.CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.BinID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BinID, "b.BinID");
                    if (Filter.BinNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BinNumber, "b.BinNumber");
                    if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "b.LocationID");
                }
                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "BinID" : Utility.CustomSorting.GetSortExpression(typeof(Bin), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Bin(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
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
