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
    public class AspNetUserLogins : BaseClass
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }

        public AspNetUserLogins()
        {
        }
        public AspNetUserLogins(string LoginProvider)
        {
            this.LoginProvider = LoginProvider;
            this.Load();
        }

        public AspNetUserLogins(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {        
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AspNetUserLogins (NOLOCK) " +
                         "WHERE LoginProvider=" + Database.HandleQuote(LoginProvider);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("LoginProvider=" + LoginProvider + " is not found");
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
        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("LoginProvider")) LoginProvider = Convert.ToString(objRow["LoginProvider"]);
                if (objColumns.Contains("ProviderKey")) ProviderKey = Convert.ToString(objRow["ProviderKey"]);
                if (objColumns.Contains("ProviderDisplayName")) ProviderDisplayName = Convert.ToString(objRow["ProviderDisplayName"]);
                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);

                if (string.IsNullOrEmpty(LoginProvider)) throw new Exception("Missing LoginProvider in the datarow");
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
                if (string.IsNullOrEmpty(LoginProvider)) throw new Exception("LoginProvider is required");
                if (string.IsNullOrEmpty(ProviderKey)) throw new Exception("ProviderKey is required");
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");

                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["LoginProvider"] = LoginProvider;
                dicParam["ProviderDisplayName"] = ProviderDisplayName;
                dicParam["ProviderKey"] = ProviderKey;

                LoginProvider = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUserLogins"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(LoginProvider)) throw new Exception("LoginProvider is required");
                if (string.IsNullOrEmpty(ProviderKey)) throw new Exception("ProviderKey is required");
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");

                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["LoginProvider"] = LoginProvider;
                dicParam["ProviderDisplayName"] = ProviderDisplayName;
                dicParam["ProviderKey"] = ProviderKey;

                dicWParam["LoginProvider"] = LoginProvider;
                dicWParam["ProviderKey"] = ProviderKey;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUserLogins"), objConn, objTran);

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

                dicDParam["LoginProvider"] = LoginProvider;
                dicDParam["ProviderKey"] = ProviderKey;

                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserLogins"), objConn, objTran);
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
                     "FROM AspNetUserLogins (NOLOCK) p " +
                     "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(LoginProvider)) strSQL += "AND p.LoginProvider<>" + Database.HandleQuote(LoginProvider);
            return Database.HasRows(strSQL);
        }

        public static AspNetUserLogins GetAspNetUserLogins(string CompanyID, AspNetUserLoginsFilter Filter)
        {
            List<AspNetUserLogins> objAspNetUserLoginss = null;
            AspNetUserLogins objReturn = null;

            try
            {
                objAspNetUserLoginss = GetAspNetUserLoginss(CompanyID, Filter);
                if (objAspNetUserLoginss != null && objAspNetUserLoginss.Count >= 1) objReturn = objAspNetUserLoginss[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetUserLoginss = null;
            }
            return objReturn;
        }

        public static List<AspNetUserLogins> GetAspNetUserLoginss(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetUserLoginss(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetUserLogins> GetAspNetUserLoginss(string CompanyID, AspNetUserLoginsFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserLoginss(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUserLogins> GetAspNetUserLoginss(string CompanyID, AspNetUserLoginsFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserLoginss(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUserLogins> GetAspNetUserLoginss(string CompanyID, AspNetUserLoginsFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUserLogins> objReturn = null;
            AspNetUserLogins objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUserLogins>();

                strSQL = "SELECT * " +
                         "FROM AspNetUserLogins (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.LoginProvider != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LoginProvider, "LoginProvider");
                    if (Filter.ProviderKey != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ProviderKey, "ProviderKey");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserLogins), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUserLogins(objData.Tables[0].Rows[i]);
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

