using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class AddressCountry : BaseClass
    {
        public string CountryID { get; set; }


        public bool IsNew { get { return string.IsNullOrEmpty(CountryID); } }

        public string CountryName { get; set; }

        public string? USPSCountryName { get; set; }

        public bool IsEligibleForPLTFedEX { get; set; }

        public string? EEL_PFC { get; set; }

        public AddressCountry()
        {

        }

        public AddressCountry(string countryID)
        {
            this.CountryID = countryID;
        }

        public AddressCountry(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("CountryID")) CountryID = Convert.ToString(objRow["CountryID"]);
                if (objColumns.Contains("CountryName")) CountryName = Convert.ToString(objRow["CountryName"]);
                if (objColumns.Contains("USPSCountryName")) USPSCountryName = Convert.ToString(objRow["USPSCountryName"]);
                if (objColumns.Contains("IsEligibleForPLTFedEX")) IsEligibleForPLTFedEX = Convert.ToBoolean(objRow["IsEligibleForPLTFedEX"]);
                if (objColumns.Contains("EEL_PFC")) EEL_PFC = Convert.ToString(objRow["EEL_PFC"]);

                if (string.IsNullOrEmpty(CountryID)) throw new Exception("Missing CountryID in the datarow");
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
                if (string.IsNullOrEmpty(CountryID)) throw new Exception("CountryID is required");
                if (string.IsNullOrEmpty(CountryName)) throw new Exception("CountryName is required");
                if (string.IsNullOrEmpty(USPSCountryName)) throw new Exception("USPSCountryName is required");
                //if (!IsNew) throw new Exception("Create cannot be performed, AddressCountry already exists");

                dicParam["CountryID"] = CountryID;
                dicParam["CountryName"] = CountryName;
                dicParam["USPSCountryName"] = USPSCountryName;
                dicParam["IsEligibleForPLTFedEX"] = IsEligibleForPLTFedEX;
                dicParam["EEL_PFC"] = EEL_PFC;
                Database.ExecuteSQL(Database.GetInsertSQL(dicParam, "AddressCountry"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(CountryID)) throw new Exception("CountryID is required");
                if (string.IsNullOrEmpty(CountryName)) throw new Exception("CountryName is required");
                if (string.IsNullOrEmpty(USPSCountryName)) throw new Exception("USPSCountryName is required");
                if (IsNew) throw new Exception("Update cannot be performed, CountryID is missing");

                dicParam["CountryName"] = CountryName;
                dicParam["USPSCountryName"] = USPSCountryName;
                dicParam["IsEligibleForPLTFedEX"] = IsEligibleForPLTFedEX;
                dicParam["EEL_PFC"] = EEL_PFC;

                dicWParam["CountryID"] = CountryID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AddressCountry"), objConn, objTran);
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
                if (string.IsNullOrEmpty(CountryID)) throw new Exception("Delete cannot be performed, CountryID is missing");
                dicDParam["CountryID"] = CountryID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AddressCountry"), objConn, objTran);
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


        public static List<AddressCountry> GetAddressCountries()
        {
            int intTotalCount = 0;
            return GetAddressCountries(null, null, null, out intTotalCount);
        }

        public static List<AddressCountry> GetAddressCountries(AddressCountryFilter Filter)
        {
            int intTotalCount = 0;
            return GetAddressCountries(Filter, null, null, out intTotalCount);
        }

        public static List<AddressCountry> GetAddressCountries(AddressCountryFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAddressCountries(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AddressCountry> GetAddressCountries(AddressCountryFilter Filter, string SortExpression, bool SortAscending,
           int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AddressCountry> objReturn = null;
            AddressCountry objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AddressCountry>();

                strSQL = "SELECT a.* " +
                         "FROM AddressCountry (NOLOCK) a ";

                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CountryID != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CountryID, "a.CountryID");
                    if (Filter.CountryName != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CountryName, "a.CountryName");
                    if (Filter.USPSCountryName != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.USPSCountryName, "a.USPSCountryName");
                    if (Filter.IsEligibleForPLTFedEX != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IsEligibleForPLTFedEX, "a.IsEligibleForPLTFedEX");
                    if (Filter.EEL_PFC != null)
                        strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EEL_PFC, "a.EEL_PFC");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "CountryID"
                            : Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AddressCountry(objData.Tables[0].Rows[i]);
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
            }

            return objReturn;
        }
    }


}
