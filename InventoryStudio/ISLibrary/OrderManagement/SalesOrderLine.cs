using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class SalesOrderLine : BaseClass
    {
        public string SalesOrderLineID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(SalesOrderLineID); } }


        public string CompanyID { get; set; }

        public string SalesOrderID { get; set; }

        public string? LocationID { get; set; }

        public string? ItemID { get; set; }

        public string? ParentSalesOrderLineID { get; set; }

        public string? ItemSKU { get; set; }

        public string? ItemName { get; set; }

        public string? ItemImageURL { get; set; }

        public string? ItemUPC { get; set; }

        public string? Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal? QuantityCommitted { get; set; }

        public decimal? QuantityShipped { get; set; }

        public string? ItemUnitID { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TaxRate { get; set; }

        public string? Status { get; set; }

        public string? ExternalID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public SalesOrderLine()
        {

        }

        public SalesOrderLine(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public SalesOrderLine(string CompanyID, string SalesOrderLineID)
        {
            this.CompanyID = CompanyID;
            this.SalesOrderLineID = SalesOrderLineID;
            Load();
        }

        public SalesOrderLine(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ParentSalesOrderLineID")) ParentSalesOrderLineID = Convert.ToString(objRow["ParentSalesOrderLineID"]);
                if (objColumns.Contains("ItemSKU")) ItemSKU = Convert.ToString(objRow["ItemSKU"]);
                if (objColumns.Contains("ItemName")) ItemName = Convert.ToString(objRow["ItemName"]);
                if (objColumns.Contains("ItemImageURL")) ItemImageURL = Convert.ToString(objRow["ItemImageURL"]);
                if (objColumns.Contains("ItemUPC")) ItemUPC = Convert.ToString(objRow["ItemUPC"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);
                if (objColumns.Contains("Quantity") && objRow["Quantity"] != DBNull.Value) Quantity = Convert.ToDecimal(objRow["Quantity"]);
                if (objColumns.Contains("Quantity") && objRow["QuantityCommitted"] != DBNull.Value) QuantityCommitted = Convert.ToDecimal(objRow["QuantityCommitted"]);
                if (objColumns.Contains("Quantity") && objRow["QuantityShipped"] != DBNull.Value) QuantityShipped = Convert.ToDecimal(objRow["QuantityShipped"]);
                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);
                if (objColumns.Contains("Quantity") && objRow["UnitPrice"] != DBNull.Value) UnitPrice = Convert.ToDecimal(objRow["UnitPrice"]);
                if (objColumns.Contains("Quantity") && objRow["TaxRate"] != DBNull.Value) TaxRate = Convert.ToDecimal(objRow["TaxRate"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
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


        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.* " +
                         "FROM SalesOrderLine s (NOLOCK) " +
                         "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND s.SalesOrderLineID = " + Database.HandleQuote(SalesOrderLineID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SalesOrderLine is not found");
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
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if (Quantity == 0) throw new Exception("Quantity is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, SalesOrderLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["LocationID"] = LocationID;
                dicParam["ItemID"] = ItemID;
                dicParam["ParentSalesOrderLineID"] = ParentSalesOrderLineID;
                dicParam["ItemSKU"] = ItemSKU;
                dicParam["ItemName"] = ItemName;
                dicParam["ItemImageURL"] = ItemImageURL;
                dicParam["ItemUPC"] = ItemUPC;
                dicParam["Description"] = Description;
                dicParam["Quantity"] = Quantity;
                dicParam["QuantityCommitted"] = QuantityCommitted;
                dicParam["QuantityShipped"] = QuantityShipped;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["TaxRate"] = TaxRate;
                dicParam["Status"] = Status;
                dicParam["ExternalID"] = ExternalID;
                dicParam["TaxRate"] = TaxRate;
                dicParam["TaxRate"] = TaxRate;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;

                SalesOrderLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SalesOrderLine"), objConn, objTran).ToString();

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
                     "FROM SalesOrderLine (NOLOCK) c " +
                     "WHERE c.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND c.SalesOrderLineID=" + Database.HandleQuote(SalesOrderLineID) +
                     "AND c.SalesOrderID=" + Database.HandleQuote(SalesOrderID);
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
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if (Quantity == 0) throw new Exception("Quantity is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, SalesOrderLineID already exists");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["LocationID"] = LocationID;
                dicParam["ItemID"] = ItemID;
                dicParam["ParentSalesOrderLineID"] = ParentSalesOrderLineID;
                dicParam["ItemSKU"] = ItemSKU;
                dicParam["ItemName"] = ItemName;
                dicParam["ItemImageURL"] = ItemImageURL;
                dicParam["ItemUPC"] = ItemUPC;
                dicParam["Description"] = Description;
                dicParam["Quantity"] = Quantity;
                dicParam["QuantityCommitted"] = QuantityCommitted;
                dicParam["QuantityShipped"] = QuantityShipped;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["TaxRate"] = TaxRate;
                dicParam["Status"] = Status;
                dicParam["ExternalID"] = ExternalID;
                dicParam["TaxRate"] = TaxRate;
                dicParam["TaxRate"] = TaxRate;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
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
                if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("Delete cannot be performed, SalesOrderLineID is missing");
                dicDParam["SalesOrderLineID"] = SalesOrderLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SalesOrderLine"), objConn, objTran);
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


        public static SalesOrderLine GetSalesOrderLine(string CompanyID, SalesOrderLineFilter Filter)
        {
            List<SalesOrderLine> objSalesOrders = null;
            SalesOrderLine objReturn = null;

            try
            {
                objSalesOrders = GetSalesOrderLines(CompanyID, Filter);
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

        public static List<SalesOrderLine> GetSalesOrderLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetSalesOrderLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<SalesOrderLine> GetSalesOrderLines(string CompanyID, SalesOrderLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetSalesOrderLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<SalesOrderLine> GetSalesOrderLines(string CompanyID, SalesOrderLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrderLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrderLine> GetSalesOrderLines(string CompanyID, SalesOrderLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrderLine> objReturn = null;
            SalesOrderLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrderLine>();

                strSQL = "SELECT s.* " +
                         "FROM SalesOrderLine (NOLOCK) s " +
                         "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.SalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderLineID, "s.SalesOrderLineID");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "s.SalesOrderID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderLineID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrderLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrderLine(objData.Tables[0].Rows[i]);
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
