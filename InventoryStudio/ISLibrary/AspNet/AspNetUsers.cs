using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetUsers : BaseClass
    {
        public string Id { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }
        public string Status { get; set; }
        public string UserType { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockedEnd { get; set; }
        public bool LockedoutEnabled { get; set; }
        public string AccessFailedCount { get; set; }

        public AspNetUsers()
        {
        }
        //public AspNetUsers(string CompanyID)
        //{
        //    this.CompanyID = CompanyID;
        //}

        public AspNetUsers(/*string CompanyID,*/ string AspNetUsersID)
        {
            //this.CompanyID = CompanyID;
            this.Id = AspNetUsersID;
            Load();
        }

        public AspNetUsers(DataRow objRow)
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
                         "FROM AspNetUsers (NOLOCK) " +
                         "AND Id=" + Database.HandleQuote(Id);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AspNetUsersID=" + Id + " is not found");
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
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("UserType")) UserType = Convert.ToString(objRow["UserType"]);
                if (objColumns.Contains("NormalizedUserName")) NormalizedUserName = Convert.ToString(objRow["NormalizedUserName"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("NormalizedEmail")) NormalizedEmail = Convert.ToString(objRow["NormalizedEmail"]);
                if (objColumns.Contains("EmailConfirmed") && objRow["EmailConfirmed"] != DBNull.Value) EmailConfirmed = Convert.ToBoolean(objRow["EmailConfirmed"]);
                if (objColumns.Contains("PasswordHash")) PasswordHash = Convert.ToString(objRow["PasswordHash"]);
                if (objColumns.Contains("SecurityStamp")) SecurityStamp = Convert.ToString(objRow["SecurityStamp"]);
                if (objColumns.Contains("ConcurrencyStamp")) ConcurrencyStamp = Convert.ToString(objRow["ConcurrencyStamp"]);
                if (objColumns.Contains("PhoneNumber")) PhoneNumber = Convert.ToString(objRow["PhoneNumber"]);
                if (objColumns.Contains("PhoneNumberConfirmed") && objRow["PhoneNumberConfirmed"] != DBNull.Value) EmailConfirmed = Convert.ToBoolean(objRow["PhoneNumberConfirmed"]);
                if (objColumns.Contains("TwoFactorEnabled") && objRow["TwoFactorEnabled"] != DBNull.Value) TwoFactorEnabled = Convert.ToBoolean(objRow["TwoFactorEnabled"]);
                if (objColumns.Contains("LockedEnd") && objRow["LockedEnd"] != DBNull.Value) LockedEnd = Convert.ToDateTime(objRow["LockedEnd"]);
                if (objColumns.Contains("LockedoutEnabled") && objRow["LockedoutEnabled"] != DBNull.Value) LockedoutEnabled = Convert.ToBoolean(objRow["LockedoutEnabled"]);
                if (objColumns.Contains("AccessFailedCount")) AccessFailedCount = Convert.ToString(objRow["AccessFailedCount"]);

                if (string.IsNullOrEmpty(Id)) throw new Exception("Missing AspNetUsersID in the datarow");
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

            try
            {
                //if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                //if (string.IsNullOrEmpty(CustomerID)) throw new Exception("CustomerID is required");
                //if (AspNetUsersLines == null || AspNetUsersLines.Count == 0) throw new Exception("At least one sales order line is required");
                if (!IsNew) throw new Exception("Create cannot be performed, AspNetUsersID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Status"] = Status;
                dicParam["UserType"] = UserType;
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
                dicParam["LockedEnd"] = LockedEnd;
                dicParam["LockedoutEnabled"] = LockedoutEnabled;
                dicParam["AccessFailedCount"] = AccessFailedCount;

                Id = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetUsers"), objConn, objTran).ToString();

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
                //if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                //if (string.IsNullOrEmpty(CustomerID)) throw new Exception("CustomerID is required");
                //if (string.IsNullOrEmpty(PONumber)) throw new Exception("PONumber is required");
                //if (TranDate == null) throw new Exception("TranDate is required");
                //if (AspNetUsersLines == null || AspNetUsersLines.Count == 0) throw new Exception("At least one sales order line is required");
                if (IsNew) throw new Exception("Update cannot be performed, AspNetUsersID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                //if (BillToAddress.State == "CA" && ShippingAmount == 0) throw new Exception("");

                dicParam["Status"] = Status;
                dicParam["UserType"] = UserType;
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
                dicParam["LockedEnd"] = LockedEnd;
                dicParam["LockedoutEnabled"] = LockedoutEnabled;
                dicParam["AccessFailedCount"] = AccessFailedCount;

                dicWParam["AspNetUsersID"] = Id;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetUsers"), objConn, objTran);

                //foreach (AspNetUsersLine objAspNetUsersLine in AspNetUsersLines)
                //{
                //    objAspNetUsersLine.UpdatedBy = UpdatedBy;
                //    objAspNetUsersLine.Update(objConn, objTran);
                //}

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
                if (IsNew) throw new Exception("Delete cannot be performed, AspNetUsersID is missing");

                dicDParam["AspNetUsersID"] = Id;
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM AspNetUsers (NOLOCK) p " +
                     "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(Id)) strSQL += "AND p.AspNetUsersID<>" + Database.HandleQuote(Id);
            return Database.HasRows(strSQL);
        }

        public static AspNetUsers GetAspNetUsers(string CompanyID, AspNetUsersFilter Filter)
        {
            List<AspNetUsers> objAspNetUserss = null;
            AspNetUsers objReturn = null;

            try
            {
                objAspNetUserss = GetAspNetUserss(CompanyID, Filter);
                if (objAspNetUserss != null && objAspNetUserss.Count >= 1) objReturn = objAspNetUserss[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetUserss = null;
            }
            return objReturn;
        }

        public static List<AspNetUsers> GetAspNetUserss(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetUserss(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetUsers> GetAspNetUserss(string CompanyID, AspNetUsersFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetUserss(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetUsers> GetAspNetUserss(string CompanyID, AspNetUsersFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetUserss(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetUsers> GetAspNetUserss(string CompanyID, AspNetUsersFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetUsers> objReturn = null;
            AspNetUsers objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetUsers>();

                strSQL = "SELECT s.* " +
                         "FROM AspNetUsers (NOLOCK) s " +
                         "INNER JOIN Customer (NOLOCK) c ON s.CustomerID=c.CustomerID " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.CustomerID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerID, "s.CustomerID");
                    if (Filter.PONumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PONumber, "s.PONumber");
                    if (Filter.TranDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.TranDate, "s.TranDate");
                    if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "s.LocationID");
                    if (Filter.ShipToAddressID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShipToAddressID, "s.ShipToAddressID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AspNetUsersID" : Utility.CustomSorting.GetSortExpression(typeof(AspNetUsers), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetUsers(objData.Tables[0].Rows[i]);
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
