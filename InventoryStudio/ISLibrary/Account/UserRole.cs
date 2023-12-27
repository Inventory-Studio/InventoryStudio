
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
    public class UserRole : BaseClass
    {
    

        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;


        public UserRole()
        {
        }

        public UserRole(string UserId, string RoleId)
        {
            this.UserId = UserId;
            this.RoleId = RoleId;
            Load();
        }

        public UserRole(DataRow objRow)
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
                         "FROM AspNetUserRoles (NOLOCK) " +
                         "WHERE UserId=" + Database.HandleQuote(UserId.ToString())  +              
                         " AND RoleId = " + Database.HandleQuote(RoleId.ToString());
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserId=" + UserId + " is not found");
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
                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);               
                if (objColumns.Contains("RoleId")) RoleId = Convert.ToString(objRow["RoleId"]);               

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
               
                // Add parameters for all the columns in the Company table, except for identity and computed columns.
                dicParam["UserId"] = UserId;
                dicParam["RoleId"] = RoleId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                // Execute the SQL insert and get the new identity value for RoleId
                Database.ExecuteSQL(Database.GetInsertSQL(dicParam, "AspNetUserRoles"), objConn, objTran).ToString();
         


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
                dicDParam["UserId"] = UserId;
                dicDParam["RoleId"] = RoleId;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserRoles"), objConn, objTran);
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
