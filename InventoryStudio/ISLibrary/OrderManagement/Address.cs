using CLRFramework;
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
    public class Address : BaseClass
    {
        public string AddressID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(AddressID); } }

        public string CompanyID { get; set; }

        public string? FullName { get; set; }

        public string? Attention { get; set; }

        public string? CompanyName { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? Address3 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public string? CountryID { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Zone { get; set; }

        public bool IsInvalidAddress { get; set; }

        public bool IsAddressUpdated { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Address()
        {

        }

        public Address(string addressID)
        {
            this.AddressID = addressID;
            Load();
        }

        public Address(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("AddressID")) AddressID = Convert.ToString(objRow["AddressID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("FullName")) FullName = Convert.ToString(objRow["FullName"]);
                if (objColumns.Contains("Attention")) Attention = Convert.ToString(objRow["Attention"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("Address1")) Address1 = Convert.ToString(objRow["Address1"]);
                if (objColumns.Contains("Address2")) Address2 = Convert.ToString(objRow["Address2"]);
                if (objColumns.Contains("Address3")) Address3 = Convert.ToString(objRow["Address3"]);
                if (objColumns.Contains("City")) City = Convert.ToString(objRow["City"]);
                if (objColumns.Contains("State")) State = Convert.ToString(objRow["State"]);
                if (objColumns.Contains("PostalCode")) PostalCode = Convert.ToString(objRow["PostalCode"]);
                if (objColumns.Contains("CountryID")) CountryID = Convert.ToString(objRow["CountryID"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("Phone")) Phone = Convert.ToString(objRow["Phone"]);
                if (objColumns.Contains("Zone")) Zone = Convert.ToString(objRow["Zone"]);
                if (objColumns.Contains("IsInvalidAddress")) IsInvalidAddress = Convert.ToBoolean(objRow["IsInvalidAddress"]);
                if (objColumns.Contains("IsAddressUpdated")) IsAddressUpdated = Convert.ToBoolean(objRow["IsAddressUpdated"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AddressID)) throw new Exception("Missing AddressID in the datarow");
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


        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT a.* " +
                         "FROM Address a (NOLOCK) " +
                         "WHERE a.AddressID=" + Database.HandleQuote(AddressID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Address is not found");
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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Address already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["FullName"] = FullName;
                dicParam["Attention"] = Attention;
                dicParam["CompanyName"] = CompanyName;
                dicParam["Address1"] = Address1;
                dicParam["Address2"] = Address2;
                dicParam["Address3"] = Address3;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = PostalCode;
                dicParam["CountryID"] = CountryID;
                dicParam["Email"] = Email;
                dicParam["Phone"] = Phone;
                dicParam["Zone"] = Zone;
                dicParam["IsInvalidAddress"] = IsInvalidAddress;
                dicParam["IsAddressUpdated"] = IsAddressUpdated;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;

                AddressID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Address"), objConn, objTran).ToString();

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
            strSQL = "SELECT TOP 1 a.* " +
                     "FROM Address (NOLOCK) a " +
                     "WHERE a.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND a.AddressID=" + Database.HandleQuote(AddressID);
            return Database.HasRows(strSQL);
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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Create cannot be performed, Address already exists");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["FullName"] = FullName;
                dicParam["Attention"] = Attention;
                dicParam["CompanyName"] = CompanyName;
                dicParam["Address1"] = Address1;
                dicParam["Address2"] = Address2;
                dicParam["Address3"] = Address3;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = PostalCode;
                dicParam["CountryID"] = CountryID;
                dicParam["Email"] = Email;
                dicParam["Phone"] = Phone;
                dicParam["Zone"] = Zone;
                dicParam["IsInvalidAddress"] = IsInvalidAddress;
                dicParam["IsAddressUpdated"] = IsAddressUpdated;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;

                dicWParam["AddressID"] = AddressID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Address"), objConn, objTran);
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
                if (string.IsNullOrEmpty(AddressID)) throw new Exception("Delete cannot be performed, AddressID is missing");
                dicDParam["AddressID"] = AddressID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Address"), objConn, objTran);
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


        public static Address GetAddress(string CompanyID, AddressFilter Filter)
        {
            List<Address> objSalesOrders = null;
            Address objReturn = null;

            try
            {
                objSalesOrders = GetAddresses(CompanyID, Filter);
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

        public static List<Address> GetAddresses(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAddresses(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Address> GetAddresses(string CompanyID, AddressFilter Filter)
        {
            int intTotalCount = 0;
            return GetAddresses(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Address> GetAddresses(string CompanyID, AddressFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAddresses(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Address> GetAddresses(string CompanyID, AddressFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Address> objReturn = null;
            Address objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Address>();

                strSQL = "SELECT a.* " +
                         "FROM Address (NOLOCK) a " +
                         "WHERE a.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.AddressID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AddressID, "a.AddressID");
                    if (Filter.IsInvalidAddress != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IsInvalidAddress, "a.IsInvalidAddress");
                    if (Filter.IsAddressUpdated != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IsAddressUpdated, "a.IsAddressUpdated");
                    if (Filter.CreatedBy != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CreatedBy, "a.CreatedBy");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AddressID" : Utility.CustomSorting.GetSortExpression(typeof(Address), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Address(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }

                TotalRecord = objReturn.Count();
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
