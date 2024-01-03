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
    public class AspNetUserTokens : BaseClass
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public AspNetUserTokens()
        {
        }
        public AspNetUserTokens(string LoginProvider)
        {
            this.LoginProvider = LoginProvider;
            this.Load();
        }

        public AspNetUserTokens(DataRow objRow)
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
                         "FROM AspNetUserTokens (NOLOCK) " +
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
        }
        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);
                if (objColumns.Contains("LoginProvider")) LoginProvider = Convert.ToString(objRow["LoginProvider"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);

                if (string.IsNullOrEmpty(UserId)) throw new Exception("Missing UserId in the datarow");
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
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");

                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["LoginProvider"] = LoginProvider;
                dicParam["Name"] = Name;
                dicParam["Value"] = Value;

                LoginProvider = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUserTokens"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(LoginProvider)) throw new Exception("LoginProvider is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");

                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["LoginProvider"] = LoginProvider;
                dicParam["Name"] = Name;
                dicParam["Value"] = Value;

                dicWParam["UserId"] = UserId;
                dicWParam["LoginProvider"] = LoginProvider;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUserTokens"), objConn, objTran);

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

                dicDParam["UserId"] = UserId;
                dicDParam["LoginProvider"] = LoginProvider;

                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserTokens"), objConn, objTran);
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
                     "FROM AspNetUserTokens (NOLOCK) p " +
                     "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(LoginProvider)) strSQL += "AND p.LoginProvider<>" + Database.HandleQuote(LoginProvider);
            return Database.HasRows(strSQL);
        }

        public static AspNetUserTokens GetAspNetUserTokens(string CompanyID, AspNetUserTokensFilter Filter)
        {
            List<AspNetUserTokens> objAspNetUserTokenss = null;
            AspNetUserTokens objReturn = null;

            try
            {
                objAspNetUserTokenss = GetAspNetUserTokenss(CompanyID, Filter);
                if (objAspNetUserTokenss != null && objAspNetUserTokenss.Count >= 1) objReturn = objAspNetUserTokenss[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetUserTokenss = null;
            }
            return objReturn;
        }

        public static List<AspNetUserTokens> GetAspNetUserTokenss(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetUserTokenss(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetUserTokens> GetAspNetUserTokenss(string CompanyID, AspNetUserTokensFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserTokenss(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUserTokens> GetAspNetUserTokenss(string CompanyID, AspNetUserTokensFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserTokenss(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUserTokens> GetAspNetUserTokenss(string CompanyID, AspNetUserTokensFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUserTokens> objReturn = null;
            AspNetUserTokens objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUserTokens>();

                strSQL = "SELECT * " +
                         "FROM AspNetUserTokens (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.UserId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserId, "UserId");
                    if (Filter.LoginProvider != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LoginProvider, "LoginProvider");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserTokens), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUserTokens(objData.Tables[0].Rows[i]);
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