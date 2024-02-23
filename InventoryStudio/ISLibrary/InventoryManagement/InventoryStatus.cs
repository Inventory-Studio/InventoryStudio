using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Net.NetworkInformation;
using CLRFramework;
using Microsoft.Data.SqlClient;

namespace ISLibrary
{
    public class InventoryStatus : BaseClass
    {
        public string InventoryStatusID { get; set; } = string.Empty;
        public bool IsNew { get { return string.IsNullOrEmpty(InventoryStatusID); } }
        public string CompanyID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        public InventoryStatus()
        {
        }

        public InventoryStatus(string InventoryStatusID)
        {
            this.InventoryStatusID = InventoryStatusID;
            Load();
        }

        public InventoryStatus(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM InventoryStatus (NOLOCK) " +
                         "WHERE InventoryStatusID=" + Database.HandleQuote(InventoryStatusID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InventoryStatusID=" + InventoryStatusID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objData = null;
            }
            
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection? objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("InventoryStatusID")) InventoryStatusID = Convert.ToString(objRow["InventoryStatusID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InventoryStatusID)) throw new Exception("Missing InventoryStatusID in the datarow");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objColumns = null;
            }
            base.Load();
        }

        public override bool Create()
        {
            SqlConnection? objConn = null;
            SqlTransaction? objTran = null;

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
                throw new Exception(ex.Message);
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

            Hashtable? dicParam = null;

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, InventoryStatusID already exists");

                dicParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["Name"] = Name;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;
                InventoryStatusID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InventoryStatus"), objConn, objTran).ToString();

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
            SqlConnection? objConn = null;
            SqlTransaction? objTran = null;

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
                throw new Exception(ex.Message);
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

            Hashtable? dicParam = null;
            Hashtable? dicWParam = null;
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, InventoryStatusID is missing");

                dicParam = new Hashtable();
                dicWParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["Name"] = Name;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["InventoryStatusID"] = InventoryStatusID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "InventoryStatus"), objConn, objTran);

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
            SqlConnection? objConn = null;
            SqlTransaction? objTran = null;

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
                throw new Exception(ex.Message);
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

            Hashtable? dicDParam = null;

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, InventoryStatusID is missing");

                dicDParam = new Hashtable();
                dicDParam["InventoryStatusID"] = InventoryStatusID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "InventoryStatus"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dicDParam = null;
            }
            LogAuditData(enumActionType.Delete);
            return true;
        }

        public static InventoryStatus? GetInventoryStatus(InventoryStatusFilter Filter)
        {
            List<InventoryStatus>? objInventoryStatuss = null;
            InventoryStatus? objReturn = null;

            try
            {
                objInventoryStatuss = GetInventoryStatuss(Filter);
                if (objInventoryStatuss != null && objInventoryStatuss.Count >= 1) objReturn = objInventoryStatuss[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objInventoryStatuss = null;
            }
            return objReturn;
        }

        public static List<InventoryStatus> GetInventoryStatuss()
        {
            int intTotalCount = 0;
            return GetInventoryStatuss(null, null, null, out intTotalCount);
        }

        public static List<InventoryStatus> GetInventoryStatuss(InventoryStatusFilter? Filter)
        {
            int intTotalCount = 0;
            return GetInventoryStatuss(Filter, null, null, out intTotalCount);
        }

        public static List<InventoryStatus> GetInventoryStatuss(InventoryStatusFilter? Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInventoryStatuss(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<InventoryStatus> GetInventoryStatuss(InventoryStatusFilter? Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<InventoryStatus>? objReturn = null;
            InventoryStatus? objNew = null;
            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<InventoryStatus>();

                strSQL = "SELECT s.* " +
                         "FROM InventoryStatus (NOLOCK) s " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CompanyID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "CompanyID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "InventoryStatusID" : Utility.CustomSorting.GetSortExpression(typeof(InventoryStatus), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new InventoryStatus(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }
    }
}
