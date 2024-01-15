using CLRFramework;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class Company : BaseClass
    {
        public string CompanyID { get; set; } = string.Empty;

        public bool IsNew
        {
            get { return string.IsNullOrEmpty(CompanyID); }
        }

        public string ParentCompanyID { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public bool AutomateFulfillment { get; set; }
        public string ShippoAPIKey { get; set; } = string.Empty;
        public bool IncludePackingSlipOnLabel { get; set; }
        public string CompanyGUID { get; set; } = string.Empty;
        public string DefaultFulfillmentMethod { get; set; } = string.Empty;
        public string DefaultFulfillmentStrategy { get; set; } = string.Empty;
        public string DefaultAllocationStrategy { get; set; } = string.Empty;
        public int? UpdatedBy { get; set; } = null;
        public DateTime? UpdatedOn { get; set; } = null;
        public int? CreatedBy { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;


        public Company()
        {
        }

        public Company(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public Company(DataRow objRow)
        {
            Load(objRow);
        }

        protected void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                strSQL = "SELECT * " +
                         "FROM Company (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID.ToString());
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CompanyID=" + CompanyID + " is not found");
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

            base.Load();
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = objRow.Table.Columns;

            try
            {
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ParentCompanyID") && objRow["ParentCompanyID"] != DBNull.Value)
                    ParentCompanyID = Convert.ToString(objRow["ParentCompanyID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("_AutomateFulfillment"))
                    AutomateFulfillment = Convert.ToBoolean(objRow["_AutomateFulfillment"]);
                if (objColumns.Contains("_ShippoAPIKey")) ShippoAPIKey = Convert.ToString(objRow["_ShippoAPIKey"]);
                if (objColumns.Contains("_IncludePackingSlipOnLabel"))
                    IncludePackingSlipOnLabel = Convert.ToBoolean(objRow["_IncludePackingSlipOnLabel"]);
                if (objColumns.Contains("CompanyGUID")) CompanyGUID = Convert.ToString(objRow["CompanyGUID"]);
                if (objColumns.Contains("DefaultFulfillmentMethod"))
                    DefaultFulfillmentMethod = Convert.ToString(objRow["DefaultFulfillmentMethod"]);
                if (objColumns.Contains("DefaultFulfillmentStrategy"))
                    DefaultFulfillmentStrategy = Convert.ToString(objRow["DefaultFulfillmentStrategy"]);
                if (objColumns.Contains("DefaultAllocationStrategy"))
                    DefaultAllocationStrategy = Convert.ToString(objRow["DefaultAllocationStrategy"]);
                if (objColumns.Contains("UpdatedBy") && objRow["UpdatedBy"] != DBNull.Value)
                    UpdatedBy = Convert.ToInt32(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToInt32(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("Missing CompanyID in the datarow");
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
                // Assume that CompanyID is an identity column and should not be included in the insert.
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");

                // Add parameters for all the columns in the Company table, except for identity and computed columns.
                dicParam["ParentCompanyID"] = ParentCompanyID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["_AutomateFulfillment"] = AutomateFulfillment;
                dicParam["_ShippoAPIKey"] = ShippoAPIKey;
                dicParam["_IncludePackingSlipOnLabel"] = IncludePackingSlipOnLabel;
                dicParam["CompanyGUID"] = CompanyGUID;
                dicParam["DefaultFulfillmentMethod"] = DefaultFulfillmentMethod;
                dicParam["DefaultFulfillmentStrategy"] = DefaultFulfillmentStrategy;
                dicParam["DefaultAllocationStrategy"] = DefaultAllocationStrategy;
                dicParam["UpdatedBy"] = UpdatedBy.HasValue ? (object)UpdatedBy.Value : DBNull.Value;
                dicParam["UpdatedOn"] = (UpdatedOn.HasValue && UpdatedOn.Value != DateTime.MinValue)
                    ? (object)UpdatedOn.Value
                    : DBNull.Value;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;

                // Execute the SQL insert and get the new identity value for CompanyID
                CompanyID = Database
                    .ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Company"), objConn, objTran).ToString();


                // Load the newly created company data
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
                // Update the company data. Assume CompanyID is the unique identifier for the record.
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (IsNew) throw new Exception("Update cannot be performed on a new record.");

                // Add parameters for all the columns in the Company table that should be updated.
                dicParam["ParentCompanyID"] = ParentCompanyID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["_AutomateFulfillment"] = AutomateFulfillment;
                dicParam["_ShippoAPIKey"] = ShippoAPIKey;
                dicParam["_IncludePackingSlipOnLabel"] = IncludePackingSlipOnLabel;
                dicParam["CompanyGUID"] = CompanyGUID;
                dicParam["DefaultFulfillmentMethod"] = DefaultFulfillmentMethod;
                dicParam["DefaultFulfillmentStrategy"] = DefaultFulfillmentStrategy;
                dicParam["DefaultAllocationStrategy"] = DefaultAllocationStrategy;
                dicParam["UpdatedBy"] = UpdatedBy.HasValue ? (object)UpdatedBy.Value : DBNull.Value;
                dicParam["UpdatedOn"] = DateTime.Now;
                // Where clause parameters
                dicWParam["CompanyID"] = CompanyID;

                // Execute the SQL update and assign the result back to CompanyID
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Company"), objConn, objTran);

                // Load the updated company data
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
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, CompanyID is missing");

                // Where clause parameters
                dicDParam["CompanyID"] = CompanyID;

                // Execute the SQL delete
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Company"), objConn, objTran);
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


        public static List<Company> GetCompanies()
        {
            int intTotalCount = 0;
            return GetCompanies(null, null, null, out intTotalCount);
        }

        public static List<Company> GetCompanies(CompanyFilter Filter)
        {
            int intTotalCount = 0;
            return GetCompanies(Filter, null, null, out intTotalCount);
        }

        public static List<Company> GetCompanies(CompanyFilter Filter, int? PageSize, int? PageNumber,
            out int TotalRecord)
        {
            return GetCompanies(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Company> GetCompanies(CompanyFilter Filter, string SortExpression, bool SortAscending,
            int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Company> objReturn = null;
            Company objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Company>();

                strSQL = "SELECT c.* " +
                         "FROM Company (NOLOCK) c ";

                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CompanyID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "CompanyID");
                    if (Filter.ParentCompanyID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentCompanyID, "ParentCompanyID");
                    if (Filter.CreatedBy != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CreatedBy, "CreatedBy");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "CompanyID"
                            : Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Company(objData.Tables[0].Rows[i]);
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