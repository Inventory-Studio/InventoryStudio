﻿using CLRFramework;
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
    public class Client : BaseClass
    {
        public string ClientID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(ClientID); } }

        public string CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string EmailAddress { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Client()
        {

        }

        public Client(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public Client(string CompanyID, string ClientID)
        {
            this.CompanyID = CompanyID;
            this.ClientID = ClientID;
            Load();
        }

        public Client(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("ClientID")) ClientID = Convert.ToString(objRow["ClientID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("EmailAddress")) EmailAddress = Convert.ToString(objRow["EmailAddress"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ClientID)) throw new Exception("Missing ClientID in the datarow");
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
                strSQL = "SELECT c.* " +
                         "FROM Client c (NOLOCK) " +
                         "WHERE c.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND c.ClientID = " + Database.HandleQuote(ClientID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Client is not found");
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
            return base.Create();
        }

        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();
            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(EmailAddress)) throw new Exception("EmailAddress is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Client already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["CreatedOn"] = DateTime.Now;
                dicParam["CreatedBy"] = CreatedBy;
                ClientID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Client"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(EmailAddress)) throw new Exception("EmailAddress is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ClientID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["ClientID"] = ClientID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Client"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ClientID is missing");
                dicDParam["ClientID"] = ClientID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Client"), objConn, objTran);
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
            strSQL = "SELECT TOP 1 c.* " +
                     "FROM Client (NOLOCK) c " +
                     "WHERE c.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND c.ClientID=" + Database.HandleQuote(ClientID);
            return Database.HasRows(strSQL);
        }

        public static List<Client> GetClients(string CompanyID)
        {
            int intTotalCount = 0;
            return GetClients(CompanyID, null, null, null, out intTotalCount);
        }

        public static Client GetClient(string CompanyID, ClientFilter Filter)
        {
            List<Client> objSalesOrders = null;
            Client objReturn = null;

            try
            {
                objSalesOrders = GetClients(CompanyID, Filter);
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



        public static List<Client> GetClients(string CompanyID, ClientFilter Filter)
        {
            int intTotalCount = 0;
            return GetClients(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Client> GetClients(string CompanyID, ClientFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetClients(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Client> GetClients(string CompanyID, ClientFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Client> objReturn = null;
            Client objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;
                objReturn = new List<Client>();
                strSQL = "SELECT c.* " +
                         "FROM Client (NOLOCK) c " +
                         "WHERE c.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.CompanyName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyName, "c.CompanyName");
                    if (Filter.EmailAddress != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmailAddress, "c.EmailAddress");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ClientID" : Utility.CustomSorting.GetSortExpression(typeof(Client), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Client(objData.Tables[0].Rows[i]);
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