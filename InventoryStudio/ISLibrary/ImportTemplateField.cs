using CLRFramework;
using ISLibrary.OrderManagement;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class ImportTemplateField : BaseClass
    {
        public string ImportTemplateFieldID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(ImportTemplateFieldID); } }

        public string CompanyID { get; set; }

        public string ImportTemplateID { get; set; }

        public string SourceField { get; set; }

        public string DestinationTable { get; set; }

        public string DestinationField { get; set; }

        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }

        public ImportTemplateField()
        {

        }
        public ImportTemplateField(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public ImportTemplateField(string CompanyID, string ImportTemplateFieldID)
        {
            this.CompanyID = CompanyID;
            this.ImportTemplateFieldID = ImportTemplateFieldID;
            Load();
        }

        public ImportTemplateField(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("ImportTemplateFieldID")) ImportTemplateFieldID = Convert.ToString(objRow["ImportTemplateFieldID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ImportTemplateID")) ImportTemplateID = Convert.ToString(objRow["ImportTemplateID"]);
                if (objColumns.Contains("SourceField")) SourceField = Convert.ToString(objRow["SourceField"]);
                if (objColumns.Contains("DestinationTable")) DestinationTable = Convert.ToString(objRow["DestinationTable"]);
                if (objColumns.Contains("DestinationField")) DestinationField = Convert.ToString(objRow["DestinationField"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (string.IsNullOrEmpty(ImportTemplateFieldID)) throw new Exception("Missing ImportTemplateFieldID in the datarow");
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

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT i.* " +
                         "FROM ImportTemplateField i (NOLOCK) " +
                         "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND i.ImportTemplateFieldID = " + Database.HandleQuote(ImportTemplateFieldID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ImportTemplateField is not found");
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
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ImportTemplateID)) throw new Exception("ImportTemplateID is required");
                if (string.IsNullOrEmpty(SourceField)) throw new Exception("SourceField is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Client already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["ImportTemplateID"] = ImportTemplateID;
                dicParam["SourceField"] = SourceField;
                dicParam["DestinationTable"] = DestinationTable;
                dicParam["DestinationField"] = DestinationField;
                dicParam["CreatedOn"] = DateTime.Now;
                dicParam["CreatedBy"] = CreatedBy;
                ImportTemplateID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ImportTemplateField"), objConn, objTran).ToString();
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
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ImportTemplateID)) throw new Exception("ImportTemplateID is required");
                if (string.IsNullOrEmpty(SourceField)) throw new Exception("SourceField is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ClientID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["ImportTemplateID"] = ImportTemplateID;
                dicParam["SourceField"] = SourceField;
                dicParam["DestinationTable"] = DestinationTable;
                dicParam["DestinationField"] = DestinationField;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["ImportTemplateFieldID"] = ImportTemplateFieldID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ImportTemplateField"), objConn, objTran);
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
            base.Update();
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
                if (IsNew) throw new Exception("Delete cannot be performed, ImportTemplateFieldID is missing");
                dicDParam["ImportTemplateFieldID"] = ImportTemplateFieldID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ImportTemplateField"), objConn, objTran);
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
            strSQL = "SELECT TOP 1 i.* " +
                     "FROM ImportTemplateField (NOLOCK) i " +
                     "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND i.ImportTemplateFieldID=" + Database.HandleQuote(ImportTemplateFieldID);
            return Database.HasRows(strSQL);
        }

        public static List<ImportTemplateField> GetImportTemplateFields(string CompanyID)
        {
            int intTotalCount = 0;
            return GetImportTemplateFields(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ImportTemplateField> GetImportTemplateFields(string CompanyID, ImportTemplateFieldFilter Filter)
        {
            int intTotalCount = 0;
            return GetImportTemplateFields(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ImportTemplateField> GetImportTemplateFields(string CompanyID, ImportTemplateFieldFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetImportTemplateFields(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }
        public static List<ImportTemplateField> GetImportTemplateFields(string CompanyID, ImportTemplateFieldFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ImportTemplateField> objReturn = null;
            ImportTemplateField objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;
                objReturn = new List<ImportTemplateField>();
                strSQL = "SELECT i.* " +
                         "FROM ImportTemplateField (NOLOCK) i " +
                         "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.ImportTemplateID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ImportTemplateID, "i.ImportTemplateID");
                    if (Filter.SourceField != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SourceField, "i.SourceField");
                    if (Filter.DestinationTable != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.DestinationTable, "i.DestinationTable");
                    if (Filter.DestinationField != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.DestinationField, "i.DestinationField");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ImportTemplateFieldID" : Utility.CustomSorting.GetSortExpression(typeof(ImportTemplateField), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ImportTemplateField(objData.Tables[0].Rows[i]);
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
                objNew = null;
            }
            return objReturn;
        }
    }
}
