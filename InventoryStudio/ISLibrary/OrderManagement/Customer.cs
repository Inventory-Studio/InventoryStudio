using CLRFramework;
using ISLibrary;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class Customer : BaseClass
    {
        public string CustomerID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(CustomerID); } }

        public string CompanyID { get; set; }

        public string? ClientID { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? ExternalID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Customer()
        {

        }

        public Customer(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public Customer(string CompanyID, string CustomerID)
        {
            this.CompanyID = CompanyID;
            this.CustomerID = CustomerID;
            Load();
        }

        public Customer(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("CustomerID")) CustomerID = Convert.ToString(objRow["CustomerID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ClientID")) ClientID = Convert.ToString(objRow["ClientID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("EmailAddress")) EmailAddress = Convert.ToString(objRow["EmailAddress"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
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
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CustomerID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["ExternalID"] = ExternalID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = UpdatedOn;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;

                CustomerID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Customer"), objConn, objTran).ToString();

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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;
            strSQL = "SELECT TOP 1 c.* " +
                     "FROM Customer (NOLOCK) c " +
                     "WHERE c.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND c.CustomerID=" + Database.HandleQuote(CustomerID);
            return Database.HasRows(strSQL);
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
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Create cannot be performed, CustomerID already exists");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["ExternalID"] = ExternalID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = UpdatedOn;

                dicWParam["CustomerID"] = CustomerID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Customer"), objConn, objTran);
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
            if (!IsLoaded) Load();
            base.Delete();
            Hashtable dicDParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CustomerID)) throw new Exception("Delete cannot be performed, CustomerID is missing");
                dicDParam["CustomerID"] = CustomerID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Customer"), objConn, objTran);
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


        public static Customer GetCustomer(string CompanyID, CustomerFilter Filter)
        {
            List<Customer> objSalesOrders = null;
            Customer objReturn = null;

            try
            {
                objSalesOrders = GetCustomers(CompanyID, Filter);
                if (objSalesOrders != null && objSalesOrders.Count >= 1) objReturn = objSalesOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrders = null;
            }
            return objReturn;
        }

        public static List<Customer> GetCustomers(string CompanyID)
        {
            int intTotalCount = 0;
            return GetCustomers(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Customer> GetCustomers(string CompanyID, CustomerFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomers(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Customer> GetCustomers(string CompanyID, CustomerFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomers(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Customer> GetCustomers(string CompanyID, CustomerFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Customer> objReturn = null;
            Customer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Customer>();

                strSQL = "SELECT c.* " +
                         "FROM Customer (NOLOCK) c " +
                         "WHERE c.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.CustomerID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerID, "c.CustomerID");
                    if (Filter.CreatedBy != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CreatedBy, "c.CreatedBy");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CustomerID" : Utility.CustomSorting.GetSortExpression(typeof(Customer), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Customer(objData.Tables[0].Rows[i]);
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