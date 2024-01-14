using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.AspNet
{
    public class AspNetUserInvites : BaseClass
    {
        public string UserInviteId { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(UserInviteId); } }

        public string Code { get; set; }

        public string Email { get; set; }

        public bool IsAccepted { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public AspNetUserInvites()
        {

        }

        public AspNetUserInvites(string userInviteId)
        {
            this.UserInviteId = userInviteId;
            this.Load();
        }

        public AspNetUserInvites(DataRow objRow)
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
                         "FROM AspNetUserInvites (NOLOCK) " +
                         "WHERE UserInviteId=" + Database.HandleQuote(UserInviteId);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserInviteId=" + UserInviteId + " is not found");
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

                if (objColumns.Contains("UserInviteId")) UserInviteId = Convert.ToString(objRow["UserInviteId"]);
                if (objColumns.Contains("Code")) Code = Convert.ToString(objRow["Code"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("UserId")) UserId = Convert.ToString(objRow["UserId"]);
                if (objColumns.Contains("IsAccepted")) IsAccepted = Convert.ToBoolean(objRow["IsAccepted"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = (DateTimeOffset)objRow["CreatedOn"];
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = (DateTimeOffset)objRow["UpdatedOn"];


                if (string.IsNullOrEmpty(UserInviteId)) throw new Exception("Missing UserInviteId in the datarow");
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
                if (string.IsNullOrEmpty(Email)) throw new Exception("Email is required");
                if (string.IsNullOrEmpty(Code)) throw new Exception("Code is required");
                if (string.IsNullOrEmpty(UserId)) throw new Exception("UserId is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Email"] = Email;
                dicParam["Code"] = Code;
                dicParam["IsAccepted"] = IsAccepted;
                dicParam["UserId"] = UserId;
                dicParam["CreatedOn"] = DateTime.UtcNow;
                UserInviteId = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUserInvites"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(UserInviteId)) throw new Exception("UserInviteId is required");
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["IsAccepted"] = IsAccepted;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["UserInviteId"] = UserInviteId;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUserInvites"), objConn, objTran);

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
                dicDParam["UserInviteId"] = UserInviteId;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUserInvites"), objConn, objTran);
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
                     "FROM AspNetUserInvites (NOLOCK) p " +
                     "WHERE 1=1 ";
            strSQL += " AND p.UserId=" + Database.HandleQuote(UserId);
            strSQL += " AND p.Code=" + Database.HandleQuote(Code);
            strSQL += " AND p.Email=" + Database.HandleQuote(Email);
            return Database.HasRows(strSQL);
        }

        public static List<AspNetUserInvites> GetAspNetUserInvites()
        {
            int intTotalCount = 0;
            return GetAspNetUserInvites(null, null, null, out intTotalCount);
        }
        public static List<AspNetUserInvites> GetAspNetUserInvites(AspNetUserInvitesFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserInvites(Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUserInvites> GetAspNetUserInvites(AspNetUserInvitesFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserInvites(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUserInvites> GetAspNetUserInvites(AspNetUserInvitesFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUserInvites> objReturn = null;
            AspNetUserInvites objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUserInvites>();

                strSQL = "SELECT * " +
                         "FROM AspNetUserInvites (NOLOCK) " +
                         "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.UserId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserId, "UserId");
                    if (Filter.UserInviteId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInviteId, "UserInviteId");
                    if (Filter.Code != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Code, "Code");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.IsAccepted != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IsAccepted, "IsAccepted");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUserCompany), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUserInvites(objData.Tables[0].Rows[i]);
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
