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
    public class AspNetAccessTokens : BaseClass
    {
        public string AccessTokenID { get; set; }

        public bool IsNew
        {
            get { return string.IsNullOrEmpty(AccessTokenID); }
        }

        public string ApplicationName { get; set; }

        public string TokenName { get; set; }

        public string Token { get; set; }

        public string Secret { get; set; }

        public bool InActive { get; set; }

        public string RoleId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public AspNetAccessTokens()
        {
        }

        public AspNetAccessTokens(string userAccessTokenId)
        {
            this.AccessTokenID = userAccessTokenId;
            this.Load();
        }

        public AspNetAccessTokens(DataRow objRow)
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
                         "FROM AspNetAccessTokens (NOLOCK) " +
                         "WHERE AccessTokenID=" + Database.HandleQuote(AccessTokenID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AccessTokenID=" + AccessTokenID + " is not found");
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

                if (objColumns.Contains("AccessTokenID")) AccessTokenID = Convert.ToString(objRow["AccessTokenID"]);
                if (objColumns.Contains("ApplicationName"))
                    ApplicationName = Convert.ToString(objRow["ApplicationName"]);
                if (objColumns.Contains("TokenName")) TokenName = Convert.ToString(objRow["TokenName"]);
                if (objColumns.Contains("Token")) Token = Convert.ToString(objRow["Token"]);
                if (objColumns.Contains("Secret")) Secret = Convert.ToString(objRow["Secret"]);
                if (objColumns.Contains("InActive")) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("RoleId")) RoleId = Convert.ToString(objRow["RoleId"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = (DateTime)objRow["CreatedOn"];
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = (DateTime)objRow["UpdatedOn"];
                if (objColumns.Contains("UpdatedBy") && objRow["UpdatedBy"] != DBNull.Value)
                    UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (string.IsNullOrEmpty(AccessTokenID)) throw new Exception("Missing AccessTokenID in the datarow");
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
                if (string.IsNullOrEmpty(ApplicationName)) throw new Exception("ApplicationName is required");
                if (string.IsNullOrEmpty(TokenName)) throw new Exception("TokenName is required");
                if (string.IsNullOrEmpty(Token)) throw new Exception("Token is required");
                if (string.IsNullOrEmpty(Secret)) throw new Exception("Secret is required");
                if (string.IsNullOrEmpty(RoleId)) throw new Exception("RoleId is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["AccessTokenID"] = AccessTokenID;
                dicParam["ApplicationName"] = ApplicationName;
                dicParam["TokenName"] = TokenName;
                dicParam["Token"] = Token;
                dicParam["Secret"] = Secret;
                dicParam["InActive"] = InActive;
                dicParam["RoleId"] = RoleId;
                dicParam["CreatedOn"] = DateTime.UtcNow;
                dicParam["CreatedBy"] = CreatedBy;
                Database.ExecuteSQL(Database.GetInsertSQL(dicParam, nameof(AspNetAccessTokens)));
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
                if (string.IsNullOrEmpty(AccessTokenID)) throw new Exception("AccessTokenID is required");
                if (IsNew) throw new Exception("Update cannot be performed, AccessTokenID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ApplicationName"] = ApplicationName;
                dicParam["TokenName"] = TokenName;
                dicParam["Token"] = Token;
                dicParam["Secret"] = Secret;
                dicParam["InActive"] = InActive;
                dicParam["RoleId"] = RoleId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["UpdatedBy"] = UpdatedBy;

                dicWParam["AccessTokenID"] = AccessTokenID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetAccessTokens"), objConn, objTran);

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
                dicDParam["AccessTokenID"] = AccessTokenID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetAccessTokens"), objConn, objTran);
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
                     "FROM AspNetAccessTokens (NOLOCK) p " +
                     "WHERE 1=1 ";
            strSQL += " AND p.Token=" + Database.HandleQuote(Token);
            strSQL += " AND p.Secret=" + Database.HandleQuote(Secret);
            return Database.HasRows(strSQL);
        }


        public static List<AspNetAccessTokens> GetAspNetUserAccessTokens()
        {
            int intTotalCount = 0;
            return GetAspNetUserAccessTokens(null, null, null, out intTotalCount);
        }

        public static List<AspNetAccessTokens> GetAspNetUserAccessTokens(AspNetAccessTokensFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserAccessTokens(Filter, null, null, out intTotalCount);
        }

        public static List<AspNetAccessTokens> GetAspNetUserAccessTokens(AspNetAccessTokensFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserAccessTokens(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetAccessTokens> GetAspNetUserAccessTokens(AspNetAccessTokensFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetAccessTokens> objReturn = null;
            AspNetAccessTokens objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetAccessTokens>();

                strSQL = "SELECT * " +
                         "FROM AspNetAccessTokens (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.RoleId != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RoleId, "RoleId");
                    if (Filter.AccessTokenID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AccessTokenID, "AccessTokenID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "AccessTokenId"
                            : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserCompany), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetAccessTokens(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }

                TotalRecord = objReturn.Count;
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