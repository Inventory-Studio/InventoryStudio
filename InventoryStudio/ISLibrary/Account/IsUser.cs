
using CLRFramework;
using ISLibrary.Account;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    //because of EF core already have User, so this model add prefix Is
    public class IsUser : BaseClass
    {
        public string Id { get; set; } = string.Empty;

        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }

        public string Status { get; set; }

        public string UserType { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string NormalizedUserName { get; set; }

        public string Email { get; set; } = string.Empty;

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }


        private List<Company>? mCompanies = null;
        public List<Company>? Companies
        {
            get
            {
                if (mCompanies == null && !string.IsNullOrEmpty(Id))
                {

                    try
                    {
                        mCompanies = GetCompanies(Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {

                    }
                }
                return mCompanies;
            }
        }

        private List<Role>? mRoles = null;
        public List<Role>? Roles
        {
            get
            {
                if (mRoles == null && !string.IsNullOrEmpty(Id))
                {

                    try
                    {
                        mRoles = GetRoles(Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {

                    }
                }
                return mRoles;
            }
        }

        public IsUser(DataRow objRow)
        {
            Load(objRow);
        }


        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = objRow.Table.Columns;

            try
            {
                if (objColumns.Contains("Id")) Id = Convert.ToString(objRow["Id"]);
                if (objColumns.Contains("Status")) Status = objRow["Status"].ToString();
                if (objColumns.Contains("UserType")) UserType = objRow["UserType"].ToString();
                if (objColumns.Contains("UserName")) UserName = objRow["UserName"].ToString();
                if (objColumns.Contains("NormalizedUserName")) NormalizedUserName = objRow["NormalizedUserName"].ToString();
                if (objColumns.Contains("Email")) Email = objRow["Email"].ToString();
                if (objColumns.Contains("NormalizedEmail")) NormalizedEmail = objRow["NormalizedEmail"].ToString();
                if (objColumns.Contains("EmailConfirmed")) EmailConfirmed = Convert.ToBoolean(objRow["EmailConfirmed"]);
                if (objColumns.Contains("PasswordHash")) PasswordHash = objRow["PasswordHash"].ToString();
                if (objColumns.Contains("SecurityStamp")) SecurityStamp = objRow["SecurityStamp"].ToString();
                if (objColumns.Contains("ConcurrencyStamp")) ConcurrencyStamp = objRow["ConcurrencyStamp"].ToString();
                if (objColumns.Contains("PhoneNumber")) PhoneNumber = objRow["PhoneNumber"].ToString();
                if (objColumns.Contains("PhoneNumberConfirmed")) PhoneNumberConfirmed = Convert.ToBoolean(objRow["PhoneNumberConfirmed"]);
                if (objColumns.Contains("TwoFactorEnabled")) TwoFactorEnabled = Convert.ToBoolean(objRow["TwoFactorEnabled"]);
                if (objColumns.Contains("LockoutEnd") && objRow["LockoutEnd"] != DBNull.Value) LockoutEnd = Convert.ToDateTime(objRow["LockoutEnd"]);
                if (objColumns.Contains("LockoutEnabled")) LockoutEnabled = Convert.ToBoolean(objRow["LockoutEnabled"]);
                if (objColumns.Contains("AccessFailedCount")) AccessFailedCount = Convert.ToInt32(objRow["AccessFailedCount"]);
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

        public IsUser()
        {
        }

        public IsUser(string Id)
        {
            this.Id = Id;
            Load();
        }


        protected void Load()
        {
            base.Load();

            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AspNetUsers (NOLOCK) " +
                         "WHERE Id=" + Database.HandleQuote(Id.ToString());
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
                throw new Exception(ex.Message);
            }
            finally
            {
                objData = null;
            }
        }

        public static List<IsUser> GetSameCompanyUsers(string compayId)
        {
            List<IsUser> objReturn = new List<IsUser>();
            IsUser objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT u.* " +
                    "FROM AspNetUsers u " +
                    "INNER JOIN AspNetUserCompany uc ON u.Id = uc.UserId " +
                    "WHERE uc.CompanyId = " + compayId;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new IsUser(objData.Tables[0].Rows[i]);
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


        public static List<Company> GetCompanies(string userId)
        {
            List<Company> objReturn = new List<Company>();
            Company objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT u.* " +
                    "FROM Company u " +
                    "INNER JOIN AspNetUserCompany uc ON u.CompanyID = uc.CompanyId " +
                    "WHERE uc.UserId = " + userId;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Company(objData.Tables[0].Rows[i]);
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

        public static List<Role> GetRoles(string userId)
        {
            List<Role> objReturn = new List<Role>();
            Role objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT u.* " +
                    "FROM AspNetRoles u " +
                    "INNER JOIN AspNetUserRoles uc ON u.Id = uc.RoleId " +
                    "WHERE uc.UserId = " + userId;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Role(objData.Tables[0].Rows[i]);
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
            try
            {
                dicParam["Status"] = Status;
                dicParam["UserType"] = UserType;
                dicParam["UserName"] = UserName;
                dicParam["NormalizedUserName"] = NormalizedUserName;
                dicParam["Email"] = Email;
                dicParam["NormalizedEmail"] = NormalizedEmail;
                dicParam["EmailConfirmed"] = EmailConfirmed;
                dicParam["PasswordHash"] = PasswordHash;
                dicParam["SecurityStamp"] = SecurityStamp;
                dicParam["ConcurrencyStamp"] = ConcurrencyStamp;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["PhoneNumberConfirmed"] = PhoneNumberConfirmed;
                dicParam["TwoFactorEnabled"] = TwoFactorEnabled;
                dicParam["LockoutEnd"] = LockoutEnd;
                dicParam["LockoutEnabled"] = LockoutEnabled;
                dicParam["AccessFailedCount"] = AccessFailedCount;
                Id = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Id"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(Id)) throw new Exception("Id is required");
                if (IsNew) throw new Exception("Update cannot be performed on a new record.");
                dicParam["Status"] = Status;
                dicParam["UserType"] = UserType;
                dicParam["UserName"] = UserName;
                dicParam["NormalizedUserName"] = NormalizedUserName;
                dicParam["Email"] = Email;
                dicParam["NormalizedEmail"] = NormalizedEmail;
                dicParam["EmailConfirmed"] = EmailConfirmed;
                dicParam["PasswordHash"] = PasswordHash;
                dicParam["SecurityStamp"] = SecurityStamp;
                dicParam["ConcurrencyStamp"] = ConcurrencyStamp;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["PhoneNumberConfirmed"] = PhoneNumberConfirmed;
                dicParam["TwoFactorEnabled"] = TwoFactorEnabled;
                dicParam["LockoutEnd"] = LockoutEnd;
                dicParam["LockoutEnabled"] = LockoutEnabled;
                dicParam["AccessFailedCount"] = AccessFailedCount;
                dicWParam["Id"] = Id;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUsers"), objConn, objTran);
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
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetUsers"), objConn, objTran);
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


        public static List<IsUser> GetUsers()
        {
            int intTotalCount = 0;
            return GetUsers(null, null, null, out intTotalCount);
        }

        public static List<IsUser> GetUsers(UserFilter Filter)
        {
            int intTotalCount = 0;
            return GetUsers(Filter, null, null, out intTotalCount);
        }

        public static List<IsUser> GetUsers(UserFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetUsers(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<IsUser> GetUsers(UserFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<IsUser> objReturn = null;
            IsUser objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                TotalRecord = 0;
                objReturn = new List<IsUser>();
                strSQL = "SELECT u.* " +
                         "FROM AspNetUsers (NOLOCK) u ";
                strSQL += "WHERE 1=1 ";
                if (Filter != null)
                {
                    if (Filter.Id != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Id, "Id");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new IsUser(objData.Tables[0].Rows[i]);
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
