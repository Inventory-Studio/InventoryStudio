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
    public class AspNetRolePermission : BaseClass
    {
        public string RoleId { get; set; }
        public string PermissionId { get; set; }

        public AspNetRolePermission()
        {
        }
      
        public AspNetRolePermission(string RoleId, string PermissionId)
        {
            this.RoleId = RoleId;
            this.PermissionId = PermissionId;           
            this.Load();
        }

        public AspNetRolePermission(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AspNetRolePermission (NOLOCK) " +
                        "WHERE PermissionId=" + Database.HandleQuote(PermissionId.ToString()) +
                         " AND RoleId = " + Database.HandleQuote(RoleId.ToString());
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RoleId=" + RoleId + " is not found");
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
        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("RoleId")) RoleId = Convert.ToString(objRow["RoleId"]);
                if (objColumns.Contains("PermissionId")) PermissionId = Convert.ToString(objRow["PermissionId"]);
             
                if (string.IsNullOrEmpty(RoleId)) throw new Exception("Missing RoleId in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(RoleId)) throw new Exception("RoleId is required");
                if (string.IsNullOrEmpty(PermissionId)) throw new Exception("PermissionId is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["RoleId"] = RoleId;
                dicParam["PermissionId"] = PermissionId;

                Database.ExecuteSQL(Database.GetInsertSQL(dicParam, "AspNetRolePermission"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(RoleId)) throw new Exception("RoleId is required");
                if (string.IsNullOrEmpty(PermissionId)) throw new Exception("PermissionId is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["RoleId"] = RoleId;
                dicParam["PermissionId"] = PermissionId;

                dicWParam["RoleId"] = RoleId;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetRolePermission"), objConn, objTran);

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
                dicDParam["RoleId"] = RoleId;
                dicDParam["PermissionId"] = PermissionId;

                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetRolePermission"), objConn, objTran);
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

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM AspNetRolePermission (NOLOCK) p " +
                     "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(RoleId)) strSQL += "AND p.RoleId=" + Database.HandleQuote(RoleId);
            if (!string.IsNullOrEmpty(PermissionId)) strSQL += "AND p.PermissionId=" + Database.HandleQuote(PermissionId);

            return Database.HasRows(strSQL);
        }

        public static AspNetRolePermission GetAspNetRolePermission(string CompanyID, AspNetRolePermissionFilter Filter)
        {
            List<AspNetRolePermission> objAspNetRolePermissions = null;
            AspNetRolePermission objReturn = null;

            try
            {
                objAspNetRolePermissions = GetAspNetRolePermissions(CompanyID, Filter);
                if (objAspNetRolePermissions != null && objAspNetRolePermissions.Count >= 1) objReturn = objAspNetRolePermissions[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetRolePermissions = null;
            }
            return objReturn;
        }

        public static List<AspNetRolePermission> GetAspNetRolePermissions(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetRolePermissions(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetRolePermission> GetAspNetRolePermissions(string CompanyID, AspNetRolePermissionFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetRolePermissions(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetRolePermission> GetAspNetRolePermissions(string CompanyID, AspNetRolePermissionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetRolePermissions(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetRolePermission> GetAspNetRolePermissions(string CompanyID, AspNetRolePermissionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetRolePermission> objReturn = null;
            AspNetRolePermission objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetRolePermission>();

                strSQL = "SELECT * " +
                         "FROM AspNetRolePermission (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.RoleId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RoleId, "RoleId");
                    if (Filter.PermissionId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PermissionId, "PermissionId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AspNetRolePermissionID" : Utility.CustomSorting.GetSortExpression(typeof(AspNetRolePermission), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetRolePermission(objData.Tables[0].Rows[i]);
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

