using CLRFramework;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.ImportTemplateManagement
{
    public class ImportFailedRecord : BaseClass
    {
        public string ImportFailedRecordID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(ImportFailedRecordID); } }

        public string ImportResultID { get; set; }

        public string ErrorMessage { get; set; } = null!;

        public string FailedData { get; set; } = null!;

        public ImportFailedRecord()
        {

        }

        public ImportFailedRecord(string ImportFailedRecordID)
        {
            this.ImportFailedRecordID = ImportFailedRecordID;
            Load();
        }

        public ImportFailedRecord(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("ImportFailedRecordID")) ImportFailedRecordID = Convert.ToString(objRow["ImportFailedRecordID"]);
                if (objColumns.Contains("ImportResultID")) ImportResultID = Convert.ToString(objRow["ImportResultID"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("FailedData")) FailedData = Convert.ToString(objRow["FailedData"]);
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
                strSQL = "SELECT i.* " +
                         "FROM ImportFailedRecord i (NOLOCK) " +
                         "WHERE i.ImportFailedRecordID=" + Database.HandleQuote(ImportFailedRecordID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ImportFailedRecord is not found");
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
                if (string.IsNullOrEmpty(ImportResultID)) throw new Exception("ImportResultID is required");
                if (string.IsNullOrEmpty(ErrorMessage)) throw new Exception("ErrorMessage is required");
                if (string.IsNullOrEmpty(FailedData)) throw new Exception("FailedData is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ImportFailedRecord already exists");
                dicParam["ImportResultID"] = ImportResultID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["FailedData"] = FailedData;
                ImportFailedRecordID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ImportFailedRecord"), objConn, objTran).ToString();
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

        public static List<ImportFailedRecord> GetImportResults(ImportFailedRecordFilter Filter)
        {
            int intTotalCount = 0;
            return GetImportResults(Filter, null, null, out intTotalCount);
        }

        public static List<ImportFailedRecord> GetImportResults(ImportFailedRecordFilter Filter, int? PageSize, int? PageNumber,
            out int TotalRecord)
        {
            return GetImportResults(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ImportFailedRecord> GetImportResults(ImportFailedRecordFilter Filter, string SortExpression, bool SortAscending,
            int? PageSize, int? PageNumber, out int TotalRecord)
        {

            List<ImportFailedRecord> objReturn = null;
            ImportFailedRecord objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ImportFailedRecord>();

                strSQL = "SELECT i.* " +
                         "FROM ImportFailedRecord (NOLOCK) i ";
                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ImportResultID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ImportResultID, "ImportResultID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "ImportFailedRecordID"
                            : CLRFramework.Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ImportFailedRecord(objData.Tables[0].Rows[i]);
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
