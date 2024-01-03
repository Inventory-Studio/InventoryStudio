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
    public class AspNetRoleClaims : BaseClass
    {
        public string Id { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    
        public AspNetRoleClaims()
        {
        }
        public AspNetRoleClaims(string Id)
        {
            this.Id = Id;
            this.Load();
        }

        public AspNetRoleClaims(DataRow objRow)
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
                         "FROM AspNetRoleClaims (NOLOCK) " +
                         "WHERE Id=" + Database.HandleQuote(Id);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Id=" + Id + " is not found");
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

                if (objColumns.Contains("Id")) Id = Convert.ToString(objRow["Id"]);
                if (objColumns.Contains("RoleId")) RoleId = Convert.ToString(objRow["RoleId"]);
                if (objColumns.Contains("ClaimType")) ClaimType = Convert.ToString(objRow["ClaimType"]);
                if (objColumns.Contains("ClaimValue")) ClaimValue = Convert.ToString(objRow["ClaimValue"]);
           
                if (string.IsNullOrEmpty(Id)) throw new Exception("Missing Id in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["RoleId"] = RoleId;
                dicParam["ClaimType"] = ClaimType;
                dicParam["ClaimValue"] = ClaimValue;
            
                Id = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetRoleClaims"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["RoleId"] = RoleId;
                dicParam["ClaimType"] = ClaimType;
                dicParam["ClaimValue"] = ClaimValue;

                dicWParam["Id"] = Id;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetRoleClaims"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, Id is missing");

                dicDParam["Id"] = Id;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetRoleClaims"), objConn, objTran);
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
                     "FROM AspNetRoleClaims (NOLOCK) p " +
                     "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(Id)) strSQL += "AND p.Id<>" + Database.HandleQuote(Id);
            return Database.HasRows(strSQL);
        }

        public static AspNetRoleClaims GetAspNetRoleClaims(string CompanyID, AspNetRoleClaimsFilter Filter)
        {
            List<AspNetRoleClaims> objAspNetRoleClaimss = null;
            AspNetRoleClaims objReturn = null;

            try
            {
                objAspNetRoleClaimss = GetAspNetRoleClaimss(CompanyID, Filter);
                if (objAspNetRoleClaimss != null && objAspNetRoleClaimss.Count >= 1) objReturn = objAspNetRoleClaimss[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetRoleClaimss = null;
            }
            return objReturn;
        }

        public static List<AspNetRoleClaims> GetAspNetRoleClaimss(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetRoleClaimss(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetRoleClaims> GetAspNetRoleClaimss(string CompanyID, AspNetRoleClaimsFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetRoleClaimss(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetRoleClaims> GetAspNetRoleClaimss(string CompanyID, AspNetRoleClaimsFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetRoleClaimss(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetRoleClaims> GetAspNetRoleClaimss(string CompanyID, AspNetRoleClaimsFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetRoleClaims> objReturn = null;
            AspNetRoleClaims objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetRoleClaims>();

                strSQL = "SELECT * " +
                         "FROM AspNetRoleClaims (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.RoleId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RoleId, "RoleId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetRoleClaims), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetRoleClaims(objData.Tables[0].Rows[i]);
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
