
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
    public class Permission : BaseClass
    {
        public string PermissionId { get; set; } = string.Empty;

        public bool IsNew { get { return string.IsNullOrEmpty(PermissionId); } }

        public string Name { get; set; } = string.Empty;


        public Permission()
        {
        }

        public Permission(string Id)
        {
            this.PermissionId = Id;
            Load();
        }

        public Permission(DataRow objRow)
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
                         "FROM AspNetPermission (NOLOCK) " +
                         "WHERE PermissionId=" + Database.HandleQuote(PermissionId.ToString());
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PermissionId=" + PermissionId + " is not found");
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
                if (objColumns.Contains("PermissionId")) PermissionId = Convert.ToString(objRow["PermissionId"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);               

                if (string.IsNullOrEmpty(PermissionId)) throw new Exception("Missing PermissionId in the datarow");
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
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");

                // Add parameters for all the columns in the Company table, except for identity and computed columns.
                dicParam["Name"] = Name;

                // Execute the SQL insert and get the new identity value for CompanyID
                PermissionId = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetPermission"), objConn, objTran).ToString();
         


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
            base.Update();

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (IsNew) throw new Exception("Update cannot be performed, Id is missing");

                dicParam["Name"] = Name;
                dicWParam["PermissionId"] = PermissionId;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetPermission"), objConn, objTran);

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

                dicDParam["PermissionId"] = PermissionId;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetPermission"), objConn, objTran);
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

        public static List<Permission> GetPermissions()
        {
            int intTotalCount = 0;
            return GetPermissions(null, null, null, out intTotalCount);
        }

        public static List<Permission> GetPermissions(PermissionFilter Filter)
        {
            int intTotalCount = 0;
            return GetPermissions(Filter, null, null, out intTotalCount);
        }

        public static List<Permission> GetPermissions(PermissionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPermissions(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Permission> GetPermissions(PermissionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Permission> objReturn = null;
            Permission objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Permission>();

                strSQL = "SELECT s.* " +
                         "FROM AspNetPermission (NOLOCK) s ";

                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.PermissionId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PermissionId, "PermissionId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PermissionID" : Utility.CustomSorting.GetSortExpression(typeof(Permission), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Permission(objData.Tables[0].Rows[i]);
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
            }
            return objReturn;
        }


    }
}
