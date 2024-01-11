using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.ComponentModel.Design;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetUserCompany : BaseClass
    {
        public string UserCompanyId { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(UserCompanyId); } }
        public string CompanyID { get; set; }
        public string UserId { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public AspNetUserCompany()
        {
        }
        public AspNetUserCompany(string UserCompanyId)
        {
            this.UserCompanyId = UserCompanyId;
            this.Load();
        }

        public AspNetUserCompany(DataRow objRow)
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
                         "FROM AspNetUserCompany (NOLOCK) " +
                         "WHERE UserCompanyId=" + Database.HandleQuote(UserCompanyId);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserCompanyId=" + UserCompanyId + " is not found");
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

                if (objColumns.Contains("UserCompanyId")) UserCompanyId = Convert.ToString(objRow["UserCompanyId"]);
                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);
                if (objColumns.Contains("CompanyId")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn") && objRow["CreatedOn"] != DBNull.Value) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(UserCompanyId)) throw new Exception("Missing UserCompanyId in the datarow");
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
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["CompanyID"] = CompanyID;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                UserCompanyId = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUserCompany"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserId"] = UserId;
                dicParam["CompanyID"] = CompanyID;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["UserCompanyId"] = UserCompanyId;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUserCompany"), objConn, objTran);

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

                dicDParam["UserCompanyId"] = UserCompanyId;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserCompany"), objConn, objTran);
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
                     "FROM AspNetUserCompany (NOLOCK) p " +
                     "WHERE 1=1 ";
            strSQL += " AND p.UserId=" + Database.HandleQuote(UserId);
            strSQL += " AND p.CompanyId=" + Database.HandleQuote(CompanyID);

            if (!string.IsNullOrEmpty(UserCompanyId)) strSQL += "AND p.UserCompanyId<>" + Database.HandleQuote(UserCompanyId);
            return Database.HasRows(strSQL);
        }

        public static AspNetUserCompany GetAspNetUserCompany(string CompanyID, AspNetUserCompanyFilter Filter)
        {
            List<AspNetUserCompany> objAspNetUserCompanys = null;
            AspNetUserCompany objReturn = null;

            try
            {
                objAspNetUserCompanys = GetAspNetUserCompanys(CompanyID, Filter);
                if (objAspNetUserCompanys != null && objAspNetUserCompanys.Count >= 1) objReturn = objAspNetUserCompanys[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetUserCompanys = null;
            }
            return objReturn;
        }

        public static List<AspNetUserCompany> GetAspNetUserCompanys(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetUserCompanys(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetUserCompany> GetAspNetUserCompanys(string CompanyID, AspNetUserCompanyFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserCompanys(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUserCompany> GetAspNetUserCompanys(string CompanyID, AspNetUserCompanyFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserCompanys(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUserCompany> GetAspNetUserCompanys(string CompanyID, AspNetUserCompanyFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUserCompany> objReturn = null;
            AspNetUserCompany objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUserCompany>();

                strSQL = "SELECT * " +
                         "FROM AspNetUserCompany (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.UserId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserId, "UserId");
                    if (Filter.CompanyId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserId, "CompanyId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserCompany), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUserCompany(objData.Tables[0].Rows[i]);
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
