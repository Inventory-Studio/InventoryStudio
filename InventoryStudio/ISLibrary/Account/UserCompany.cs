
using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class UserCompany : BaseClass
    {
        public string UserCompanyId { get; set; } = string.Empty;

        public bool IsNew { get { return string.IsNullOrEmpty(UserCompanyId); } }

        public string UserId { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;


        public UserCompany()
        {
        }

        public UserCompany(string Id)
        {
            this.UserCompanyId = Id;
            Load();
        }

        public UserCompany(DataRow objRow)
        {
            Load(objRow);
        }

        protected void Load()
        {
            base.Load();

            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AspNetUserCompany (NOLOCK) " +
                         "WHERE UserCompanyId=" + Database.HandleQuote(UserCompanyId.ToString());
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserCompanyId=" + UserCompanyId + " is not found");
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
            DataColumnCollection objColumns = objRow.Table.Columns;

            try
            {
                if (objColumns.Contains("UserCompanyId")) UserCompanyId = Convert.ToString(objRow["UserCompanyId"]);
                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);               
                if (objColumns.Contains("CompanyId")) CompanyId = Convert.ToString(objRow["CompanyId"]);               

                if (string.IsNullOrEmpty(UserCompanyId)) throw new Exception("Missing UserCompanyId in the datarow");
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
             
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");

                // Add parameters for all the columns in the Company table, except for identity and computed columns.
                dicParam["UserId"] = UserId;
                dicParam["CompanyId"] = CompanyId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                // Execute the SQL insert and get the new identity value for CompanyID
                UserCompanyId = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUserCompany"), objConn, objTran).ToString();
         


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
                if (IsNew) throw new Exception("Delete cannot be performed, PermissionId is missing");

                dicDParam["UserId"] = UserId;
                dicDParam["ComapnyId"] = CompanyId;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserCompany"), objConn, objTran);
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



    }
}
