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
    public class AspNetUserRoles : BaseClass
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public AspNetUserRoles()
        {
        }
        public AspNetUserRoles(string UserId, string RoleId)
        {
            this.UserId = UserId;
            this.RoleId = RoleId;
            this.Load();
        }

        public AspNetUserRoles(DataRow objRow)
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
                         "FROM AspNetUserRoles (NOLOCK) " +
                          "WHERE UserId=" + Database.HandleQuote(UserId.ToString()) +
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
                if (objColumns.Contains("RoleId")) RoleId = Convert.ToString(objRow["RoleId"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

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
            base.Load();
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
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (string.IsNullOrEmpty(RoleId)) throw new Exception("RoleId is required");

                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["RoleId"] = RoleId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                Database.ExecuteSQL(Database.GetInsertSQL(dicParam, "AspNetUserRoles"), objConn, objTran).ToString();

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
            LogAuditData(enumActionType.Create);
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
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (string.IsNullOrEmpty(RoleId)) throw new Exception("RoleId is required");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["UserId"] = UserId;
                dicParam["RoleId"] = RoleId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["UserId"] = UserId;
                dicWParam["RoleId"] = RoleId;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUserRoles"), objConn, objTran);
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
            
            LogAuditData(enumActionType.Update);
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
            LogAuditData(enumActionType.Delete);
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM AspNetUserRoles (NOLOCK) p " +
                     "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(UserId)) strSQL += "AND p.UserId=" + Database.HandleQuote(UserId);
            if (!string.IsNullOrEmpty(RoleId)) strSQL += "AND p.RoleId=" + Database.HandleQuote(RoleId);
            return Database.HasRows(strSQL);
        }

        public static AspNetUserRoles GetAspNetUserRoles(string CompanyID, AspNetUserRolesFilter Filter)
        {
            List<AspNetUserRoles> objAspNetUserRoless = null;
            AspNetUserRoles objReturn = null;

            try
            {
                objAspNetUserRoless = GetAspNetUserRoless(CompanyID, Filter);
                if (objAspNetUserRoless != null && objAspNetUserRoless.Count >= 1) objReturn = objAspNetUserRoless[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetUserRoless = null;
            }
            return objReturn;
        }

        public static List<AspNetUserRoles> GetAspNetUserRoless(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetUserRoless(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetUserRoles> GetAspNetUserRoless(string CompanyID, AspNetUserRolesFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserRoless(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUserRoles> GetAspNetUserRoless(string CompanyID, AspNetUserRolesFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserRoless(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUserRoles> GetAspNetUserRoless(string CompanyID, AspNetUserRolesFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUserRoles> objReturn = null;
            AspNetUserRoles objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUserRoles>();

                strSQL = "SELECT * " +
                         "FROM AspNetUserRoles (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.UserId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserId, "UserId");
                    if (Filter.RoleId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RoleId, "RoleId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserRoles), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUserRoles(objData.Tables[0].Rows[i]);
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

