using CLRFramework;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class SalesOrderLineDetail : BaseClass
    {
        public string SalesOrderLineDetailID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(SalesOrderLineDetailID); } }

        public string CompanyID { get; set; }

        public string SalesOrderLineID { get; set; }

        public string BinID { get; set; }
        public string ItemID { get; set; }

        public decimal Quantity { get; set; }

        public string? InventoryNumber { get; set; }

        public string InventoryDetailID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        private InventoryDetail mInventoryDetail = null;
        public InventoryDetail InventoryDetail
        {
            get
            {
                InventoryDetailFilter objFilter = null;
                try
                {
                    if (mInventoryDetail == null && !string.IsNullOrEmpty(BinID) && !string.IsNullOrEmpty(ItemID))
                    {
                        objFilter = new InventoryDetailFilter();
                        objFilter.BinID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BinID.SearchString = BinID;
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        if (!string.IsNullOrEmpty(InventoryNumber))
                        {
                            objFilter.InventoryNumber = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.InventoryNumber.SearchString = InventoryNumber;
                        }
                        mInventoryDetail = InventoryDetail.GetInventoryDetail(CompanyID, objFilter);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    objFilter = null;
                }
                return mInventoryDetail;
            }
            set
            {
                mInventoryDetail = value;
            }
        }

        public SalesOrderLineDetail()
        {

        }

        public SalesOrderLineDetail(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public SalesOrderLineDetail(string CompanyID, string SalesOrderLineDetailID)
        {
            this.CompanyID = CompanyID;
            this.SalesOrderLineID = SalesOrderLineDetailID;
            Load();
        }

        public SalesOrderLineDetail(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("SalesOrderLineDetailID")) SalesOrderLineDetailID = Convert.ToString(objRow["SalesOrderLineDetailID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("BinID")) BinID = Convert.ToString(objRow["BinID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToDecimal(objRow["Quantity"]);
                if (objColumns.Contains("InventoryNumber")) InventoryNumber = Convert.ToString(objRow["InventoryNumber"]);
                if (objColumns.Contains("InventoryDetailID")) InventoryDetailID = Convert.ToString(objRow["InventoryDetailID"]);
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
                strSQL = "SELECT s.* " +
                         "FROM SalesOrderLineDetail s (NOLOCK) " +
                         "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND s.SalesOrderLineDetailID = " + Database.HandleQuote(SalesOrderLineDetailID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SalesOrderLineDetail is not found");
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
                if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("SalesOrderLineID is required");
                if (Quantity == 0) throw new Exception("Quantity is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, SalesOrderLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["BinID"] = BinID;
                dicParam["Quantity"] = Quantity;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["InventoryDetailID"] = InventoryDetailID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;
                SalesOrderLineDetailID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SalesOrderLineDetail"), objConn, objTran).ToString();
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;
            strSQL = "SELECT TOP 1 s.* " +
                     "FROM SalesOrderLineDetail (NOLOCK) s " +
                     "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND s.SalesOrderLineID=" + Database.HandleQuote(SalesOrderLineID) +
                     "AND s.SalesOrderLineDetailID=" + Database.HandleQuote(SalesOrderLineDetailID);
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
            base.Update();
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(SalesOrderLineDetailID)) throw new Exception("SalesOrderLineDetailID is required");
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("SalesOrderLineID is required");
                if (Quantity == 0) throw new Exception("Quantity is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("CreatedBy is required");

                dicParam["CompanyID"] = CompanyID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["BinID"] = BinID;
                dicParam["Quantity"] = Quantity;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["InventoryDetailID"] = InventoryDetailID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["SalesOrderLineDetailID"] = SalesOrderLineDetailID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SalesOrderLineDetail"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, SalesOrderLineDetailID is missing");
                dicDParam["SalesOrderLineDetailID"] = SalesOrderLineDetailID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SalesOrderLineDetail"), objConn, objTran);
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

        public static SalesOrderLineDetail GetSalesOrderLineDetail(string CompanyID, SalesOrderLineDetailFilter Filter)
        {
            List<SalesOrderLineDetail> objSalesOrders = null;
            SalesOrderLineDetail objReturn = null;

            try
            {
                objSalesOrders = GetSalesOrderLineDetails(CompanyID, Filter);
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

        public static List<SalesOrderLineDetail> GetSalesOrderLineDetails(string CompanyID)
        {
            int intTotalCount = 0;
            return GetSalesOrderLineDetails(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<SalesOrderLineDetail> GetSalesOrderLineDetails(string CompanyID, SalesOrderLineDetailFilter Filter)
        {
            int intTotalCount = 0;
            return GetSalesOrderLineDetails(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<SalesOrderLineDetail> GetSalesOrderLineDetails(string CompanyID, SalesOrderLineDetailFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrderLineDetails(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrderLineDetail> GetSalesOrderLineDetails(string CompanyID, SalesOrderLineDetailFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrderLineDetail> objReturn = null;
            SalesOrderLineDetail objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrderLineDetail>();

                strSQL = "SELECT s.* " +
                         "FROM SalesOrderLineDetail (NOLOCK) s " +
                         "WHERE s.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.SalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderLineID, "s.SalesOrderLineID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderLineDetailID" : CLRFramework.Utility.CustomSorting.GetSortExpression(typeof(SalesOrderLineDetail), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrderLineDetail(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }

                TotalRecord = objReturn.Count;
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
