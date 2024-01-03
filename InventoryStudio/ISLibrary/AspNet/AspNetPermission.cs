using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetPermission : BaseClass
    {
        public string PermissionId { get; set; }
        public string Name { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(PermissionId); } }
     
        public AspNetPermission()
        {
        }

        public AspNetPermission(/*string CompanyID,*/ string PermissionId)
        {
            //this.CompanyID = CompanyID;
            this.PermissionId = PermissionId;
            this.Load();
        }

        public AspNetPermission(DataRow objRow)
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
                         "FROM AspNetPermission (NOLOCK) " +
                         "WHERE PermissionId=" + Database.HandleQuote(PermissionId);
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (!IsNew) throw new Exception("Create cannot be performed, PermissionId already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Name"] = Name;

                PermissionId = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetPermission"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM AspNetPermission (NOLOCK) p " +
                     "WHERE 1=1 " +
                     "AND p.Name=" + Database.HandleQuote(Name);

            if (!string.IsNullOrEmpty(PermissionId)) strSQL += "AND p.PermissionId<>" + Database.HandleQuote(PermissionId);
            return Database.HasRows(strSQL);
        }

        public static AspNetPermission GetAspNetPermission(string CompanyID, AspNetPermissionFilter Filter)
        {
            List<AspNetPermission> objAspNetPermissions = null;
            AspNetPermission objReturn = null;

            try
            {
                objAspNetPermissions = GetAspNetPermissions(CompanyID, Filter);
                if (objAspNetPermissions != null && objAspNetPermissions.Count >= 1) objReturn = objAspNetPermissions[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetPermissions = null;
            }
            return objReturn;
        }

        public static List<AspNetPermission> GetAspNetPermissions(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetPermissions(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetPermission> GetAspNetPermissions(string CompanyID, AspNetPermissionFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetPermissions(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetPermission> GetAspNetPermissions(string CompanyID, AspNetPermissionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetPermissions(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetPermission> GetAspNetPermissions(string CompanyID, AspNetPermissionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetPermission> objReturn = null;
            AspNetPermission objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetPermission>();

                strSQL = "SELECT * " +
                         "FROM AspNetPermission (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PermissionId" : Utility.CustomSorting.GetSortExpression(typeof(AspNetPermission), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetPermission(objData.Tables[0].Rows[i]);
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
