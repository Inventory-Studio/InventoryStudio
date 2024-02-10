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
    public class ImportTemplate : BaseClass
    {
        public string ImportTemplateID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(ImportTemplateID); } }

        public string CompanyID { get; set; }

        public string TemplateName { get; set; }

        public string Type { get; set; }

        public string ImportType { get; set; }

        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }

        public ImportTemplate(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public ImportTemplate(string CompanyID, string ImportTemplateID)
        {
            this.CompanyID = CompanyID;
            this.ImportTemplateID = ImportTemplateID;
            Load();
        }

        public ImportTemplate(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("ImportTemplateID")) ImportTemplateID = Convert.ToString(objRow["ImportTemplateID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("TemplateName")) TemplateName = Convert.ToString(objRow["TemplateName"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);
                if (objColumns.Contains("ImportType")) ImportType = Convert.ToString(objRow["ImportType"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ImportTemplateID)) throw new Exception("Missing ImportTemplateID in the datarow");
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
                         "FROM ImportTemplate i (NOLOCK) " +
                         "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND i.ImportTemplateID = " + Database.HandleQuote(ImportTemplateID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ImportTemplate is not found");
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
                if (string.IsNullOrEmpty(TemplateName)) throw new Exception("TemplateName is required");
                if (string.IsNullOrEmpty(Type)) throw new Exception("Type is required");
                if (string.IsNullOrEmpty(ImportType)) throw new Exception("ImportType is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ImportTemplate already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["TemplateName"] = TemplateName;
                dicParam["Type"] = Type;
                dicParam["ImportType"] = ImportType;
                dicParam["CreatedOn"] = DateTime.Now;
                dicParam["CreatedBy"] = CreatedBy;
                ImportTemplateID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ImportTemplate"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(TemplateName)) throw new Exception("TemplateName is required");
                if (string.IsNullOrEmpty(Type)) throw new Exception("Type is required");
                if (string.IsNullOrEmpty(ImportType)) throw new Exception("ImportType is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ClientID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["TemplateName"] = TemplateName;
                dicParam["Type"] = Type;
                dicParam["ImportType"] = ImportType;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["ImportTemplateID"] = ImportTemplateID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ImportTemplate"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ImportTemplateID is missing");
                dicDParam["ImportTemplateID"] = ImportTemplateID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ImportTemplate"), objConn, objTran);
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
                     "FROM ImportTemplate (NOLOCK) i " +
                     "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND i.ImportTemplateID=" + Database.HandleQuote(ImportTemplateID);
            return Database.HasRows(strSQL);
        }

        public static List<ImportTemplate> GetImportTemplates(string CompanyID)
        {
            int intTotalCount = 0;
            return GetImportTemplates(CompanyID, null, null, null, out intTotalCount);
        }
        public static List<ImportTemplate> GetImportTemplates(string CompanyID, ImportTemplateFilter Filter)
        {
            int intTotalCount = 0;
            return GetImportTemplates(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ImportTemplate> GetImportTemplates(string CompanyID, ImportTemplateFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetImportTemplates(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ImportTemplate> GetImportTemplates(string CompanyID, ImportTemplateFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ImportTemplate> objReturn = null;
            ImportTemplate objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;
                objReturn = new List<ImportTemplate>();
                strSQL = "SELECT i.* " +
                         "FROM ImportTemplate (NOLOCK) i " +
                         "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.TemplateName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TemplateName, "i.CompanyName");
                    if (Filter.Type != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Type, "i.Type");
                    if (Filter.ImportType != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ImportType, "i.ImportType");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ImportTemplateID" : Utility.CustomSorting.GetSortExpression(typeof(ImportTemplate), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ImportTemplate(objData.Tables[0].Rows[i]);
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
