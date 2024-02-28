using CLRFramework;
using ISLibrary.OrderManagement;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class Vendor : BaseClass
    {
        public string VendorID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(VendorID); } }

        public string CompanyID { get; set; }

        public string? ClientID { get; set; }

        public string? VendorNumber { get; set; }

        public string? ExternalID { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        public Vendor()
        {

        }

        public Vendor(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public Vendor(string CompanyID, string VendorID)
        {
            this.CompanyID = CompanyID;
            this.VendorID = VendorID;
            Load();
        }

        public Vendor(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("VendorID")) VendorID = Convert.ToString(objRow["VendorID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ClientID")) ClientID = Convert.ToString(objRow["ClientID"]);
                if (objColumns.Contains("VendorNumber")) VendorNumber = Convert.ToString(objRow["VendorNumber"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("EmailAddress")) EmailAddress = Convert.ToString(objRow["EmailAddress"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
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

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                strSQL = "SELECT v.* " +
                         "FROM Vendor v (NOLOCK) " +
                         "WHERE v.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND v.VendorID = " + Database.HandleQuote(VendorID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Customer is not found");
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

        public bool Create()
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

        public bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();
            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CustomerID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["VendorNumber"] = VendorNumber;
                dicParam["ExternalID"] = ExternalID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;
                VendorID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Vendor"), objConn, objTran).ToString();

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

        public bool Update()
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

        public bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Create cannot be performed, CustomerID already exists");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["VendorNumber"] = VendorNumber;
                dicParam["ExternalID"] = ExternalID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["VendorID"] = VendorID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Vendor"), objConn, objTran);
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
            if (!IsLoaded) Load();
            base.Delete();
            Hashtable dicDParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(VendorID)) throw new Exception("Delete cannot be performed, CustomerID is missing");
                dicDParam["VendorID"] = VendorID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Vendor"), objConn, objTran);
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
            strSQL = "SELECT TOP 1 c.* " +
                     "FROM Vendor (NOLOCK) v " +
                     "WHERE v.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND v.VendorID=" + Database.HandleQuote(VendorID);
            return Database.HasRows(strSQL);
        }


        public static List<Vendor> GetCustomers(string CompanyID)
        {
            int intTotalCount = 0;
            return GetCustomers(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Vendor> GetCustomers(string CompanyID, VendorFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomers(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Vendor> GetCustomers(string CompanyID, VendorFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomers(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Vendor> GetCustomers(string CompanyID, VendorFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Vendor> objReturn = null;
            Vendor objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                TotalRecord = 0;
                objReturn = new List<Vendor>();
                strSQL = "SELECT v.* " +
                                         "FROM Vendor (NOLOCK) v " +
                                         "WHERE v.CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.VendorID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.VendorID, "v.VendorID");
                    if (Filter.VendorNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.VendorNumber, "v.VendorNumber");
                    if (Filter.CreatedBy != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CreatedBy, "v.CreatedBy");
                }
                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "VendorID" : Utility.CustomSorting.GetSortExpression(typeof(Vendor), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Vendor(objData.Tables[0].Rows[i]);
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
                objNew = null;
            }
            return objReturn;
        }
    }
}
