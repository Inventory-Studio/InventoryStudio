using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.AspNet
{
    public class AspNetUserAccessTokens : BaseClass
    {
        public string UserAccessTokenId { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(UserAccessTokenId); } }

        public string Token { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset ExpirationTime { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public AspNetUserAccessTokens()
        {

        }

        public AspNetUserAccessTokens(string userAccessTokenId)
        {
            this.UserAccessTokenId = userAccessTokenId;
            this.Load();
        }

        public AspNetUserAccessTokens(DataRow objRow)
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
                         "FROM AspNetUserAccessTokens (NOLOCK) " +
                         "WHERE UserAccessTokenId=" + Database.HandleQuote(UserAccessTokenId);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserAccessTokenId=" + UserAccessTokenId + " is not found");
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

                if (objColumns.Contains("UserAccessTokenId")) UserAccessTokenId = Convert.ToString(objRow["UserAccessTokenId"]);
                if (objColumns.Contains("Token")) Token = Convert.ToString(objRow["Token"]);
                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);
                if (objColumns.Contains("ExpirationTime") && objRow["ExpirationTime"] != DBNull.Value) ExpirationTime = (DateTimeOffset)objRow["ExpirationTime"];
                if (objColumns.Contains("CreatedOn")) CreatedOn = (DateTimeOffset)objRow["CreatedOn"];
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = (DateTimeOffset)objRow["UpdatedOn"];
                if (string.IsNullOrEmpty(UserAccessTokenId)) throw new Exception("Missing UserAccessTokenId in the datarow");
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
                if (string.IsNullOrEmpty(Token)) throw new Exception("Token is required");
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["Token"] = Token;
                dicParam["UserId"] = UserId;
                dicParam["ExpirationTime"] = ExpirationTime;
                dicParam["CreatedOn"] = DateTime.UtcNow;
                UserAccessTokenId = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUserAccessTokens"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(UserAccessTokenId)) throw new Exception("UserAccessTokenId is required");
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Token"] = Token;
                dicParam["UserId"] = UserId;
                dicParam["ExpirationTime"] = ExpirationTime;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["UserAccessTokenId"] = UserAccessTokenId;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUserAccessTokens"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, Id is missing");
                dicDParam["UserAccessTokenId"] = UserAccessTokenId;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserAccessTokens"), objConn, objTran);
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
            strSQL += " AND p.UserId=" + Database.HandleQuote(UserId);
            strSQL += " AND p.Code=" + Database.HandleQuote(Token);
            return Database.HasRows(strSQL);
        }


        public static List<AspNetUserAccessTokens> GetAspNetUserAccessTokens()
        {
            int intTotalCount = 0;
            return GetAspNetUserAccessTokens(null, null, null, out intTotalCount);
        }
        public static List<AspNetUserAccessTokens> GetAspNetUserAccessTokens(AspNetUserAccessTokensFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserAccessTokens(Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUserAccessTokens> GetAspNetUserAccessTokens(AspNetUserAccessTokensFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserAccessTokens(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUserAccessTokens> GetAspNetUserAccessTokens(AspNetUserAccessTokensFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUserAccessTokens> objReturn = null;
            AspNetUserAccessTokens objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUserAccessTokens>();

                strSQL = "SELECT * " +
                         "FROM AspNetUserAccessTokens (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.UserId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserId, "UserId");
                    if (Filter.UserAccessTokenId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserAccessTokenId, "UserAccessTokenId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserCompany), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUserAccessTokens(objData.Tables[0].Rows[i]);
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
