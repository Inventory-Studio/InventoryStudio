using CLRFramework;
using ISLibrary.OrderManagement;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.ImportTemplateManagement
{
    public class ImportResult : BaseClass
    {
        public string ImportResultID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(ImportResultID); } }

        public string ImportTemplateID { get; set; }

        public int TotalRecords { get; set; }

        public int SuccessfulRecords { get; set; }

        public int FailedRecords { get; set; }

        [DisplayName("Upload By")]
        public string UploadBy { get; set; }

        [DisplayName("Upload Time")]
        public DateTime UploadTime { get; set; }

        [DisplayName("Completion Time")]
        public DateTime? CompletionTime { get; set; }

        private List<ImportFailedRecord> mImportFailedRecord = null;

        public List<ImportFailedRecord> ImportFailedRecords
        {
            get
            {
                ImportFailedRecordFilter objFilter = null;
                try
                {
                    if (mImportFailedRecord == null && !string.IsNullOrEmpty(ImportResultID))
                    {
                        objFilter = new ImportFailedRecordFilter();
                        objFilter.ImportResultID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ImportResultID.SearchString = ImportResultID;
                        mImportFailedRecord = ImportFailedRecord.GetImportResults(objFilter);
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
                return mImportFailedRecord;
            }
            set
            {
                mImportFailedRecord = value;
            }
        }

        public ImportResult()
        {

        }

        public ImportResult(string ImportResultID)
        {
            this.ImportResultID = ImportResultID;
            Load();
        }

        public ImportResult(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("ImportResultID")) ImportResultID = Convert.ToString(objRow["ImportResultID"]);
                if (objColumns.Contains("ImportTemplateID")) ImportTemplateID = Convert.ToString(objRow["ImportTemplateID"]);
                if (objColumns.Contains("TotalRecords")) TotalRecords = Convert.ToInt32(objRow["TotalRecords"]);
                if (objColumns.Contains("SuccessfulRecords")) SuccessfulRecords = Convert.ToInt32(objRow["SuccessfulRecords"]);
                if (objColumns.Contains("FailedRecords")) FailedRecords = Convert.ToInt32(objRow["FailedRecords"]);
                if (objColumns.Contains("UpdatedBy")) UploadBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UploadTime")) UploadTime = Convert.ToDateTime(objRow["UploadTime"]);
                if (objColumns.Contains("CompletionTime") && objRow["CompletionTime"] != DBNull.Value)
                    CompletionTime = Convert.ToDateTime(objRow["CompletionTime"]);
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
                         "FROM ImportResult i (NOLOCK) " +
                         "WHERE i.ImportResultID=" + Database.HandleQuote(ImportResultID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ImportResult is not found");
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
                if (string.IsNullOrEmpty(UploadBy)) throw new Exception("UploadBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ImportResult already exists");
                dicParam["ImportTemplateID"] = ImportTemplateID;
                dicParam["TotalRecords"] = TotalRecords;
                dicParam["SuccessfulRecords"] = SuccessfulRecords;
                dicParam["FailedRecords"] = FailedRecords;
                dicParam["UploadBy"] = UploadBy;
                dicParam["UploadTime"] = UploadTime;
                dicParam["CompletionTime"] = CompletionTime;
                ImportResultID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ImportResult"), objConn, objTran).ToString();

                if (ImportFailedRecords != null)
                {
                    foreach (var objfailedRecord in ImportFailedRecords)
                    {
                        if (objfailedRecord.IsNew)
                        {
                            objfailedRecord.ImportResultID = ImportResultID;
                            objfailedRecord.Create();
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

                if (string.IsNullOrEmpty(ImportResultID)) throw new Exception("ImportResultID is required");
                if (IsNew) throw new Exception("Update cannot be performed on a new record.");
                dicParam["TotalRecords"] = TotalRecords;
                dicParam["SuccessfulRecords"] = SuccessfulRecords;
                dicParam["FailedRecords"] = FailedRecords;
                dicParam["CompletionTime"] = DateTime.Now;
                dicWParam["ImportResultID"] = ImportResultID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ImportResult"), objConn, objTran);
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


        public static List<ImportResult> GetImportResults(ImportResultFilter Filter)
        {
            int intTotalCount = 0;
            return GetImportResults(Filter, null, null, out intTotalCount);
        }

        public static List<ImportResult> GetImportResults(ImportResultFilter Filter, int? PageSize, int? PageNumber,
            out int TotalRecord)
        {
            return GetImportResults(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ImportResult> GetImportResults(ImportResultFilter Filter, string SortExpression, bool SortAscending,
            int? PageSize, int? PageNumber, out int TotalRecord)
        {

            List<ImportResult> objReturn = null;
            ImportResult objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;
                objReturn = new List<ImportResult>();
                strSQL = "SELECT ir.* FROM ImportResult (NOLOCK) AS ir LEFT JOIN ImportTemplate (NOLOCK) AS it " +
                    "ON ir.ImportTemplateID=it.ImportTemplateID WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.CompanyID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "it.CompanyID");
                    if (Filter.ImportTemplateID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ImportTemplateID, "ir.ImportTemplateID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "ImportResultID"
                            : CLRFramework.Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ImportResult(objData.Tables[0].Rows[i]);
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
