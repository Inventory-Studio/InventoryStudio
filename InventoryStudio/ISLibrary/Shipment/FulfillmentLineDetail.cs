using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using System.Security;
using System.ComponentModel.Design;

namespace ISLibrary
{
    public class FulfillmentLineDetail : BaseClass
    {
        public string FulfillmentLineDetailID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(FulfillmentLineDetailID); }
        }
        public string CompanyID { get; set; }
        public string FulfillmentLineID { get; set; }
        public string BinID { get; set; }
        public decimal Quantity { get; set; }
        public string ItemUnitID { get; set; }
        public decimal BaseQuantity { get; set; }   
        public string InventoryNumber { get; set; }
        public string InventoryDetailID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

 
        public FulfillmentLineDetail()
        {
        }

        public FulfillmentLineDetail( string CompanyID, string Id)
        {
            this.CompanyID = CompanyID;
            this.FulfillmentLineDetailID = FulfillmentLineDetailID;
            this.Load();
        }

        public FulfillmentLineDetail(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM FulfillmentLineDetail (NOLOCK) " +
                         "WHERE FulfillmentLineDetailID=" + Database.HandleQuote(FulfillmentLineDetailID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentLineDetailID=" + FulfillmentLineDetailID + " is not found");
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

                if (objColumns.Contains("FulfillmentLineDetailID")) FulfillmentLineDetailID = Convert.ToString(objRow["FulfillmentLineDetailID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("FulfillmentLineID")) FulfillmentLineID = Convert.ToString(objRow["FulfillmentLineID"]);
                if (objColumns.Contains("BinID")) BinID = Convert.ToString(objRow["BinID"]);
                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt16(objRow["Quantity"]);
                if (objColumns.Contains("BaseQuantity")) BaseQuantity = Convert.ToDecimal(objRow["BaseQuantity"]);
                if (objColumns.Contains("InventoryNumber")) InventoryNumber = Convert.ToString(objRow["InventoryNumber"]);
                if (objColumns.Contains("InventoryDetailID")) InventoryDetailID = Convert.ToString(objRow["InventoryDetailID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(FulfillmentLineDetailID)) throw new Exception("Missing FulfillmentLineDetailID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, FulfillmentLineDetailID already exists");


                dicParam["CompanyID"] = CompanyID;
                dicParam["FulfillmentLineID"] = FulfillmentLineID;
                dicParam["BinID"] = BinID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["BaseQuantity"] = BaseQuantity;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["InventoryDetailID"] = InventoryDetailID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                FulfillmentLineDetailID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "FulfillmentLineDetail"), objConn, objTran)
                    .ToString();


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


        public static FulfillmentLineDetail GetFulfillmentLineDetail(string CompanyID, FulfillmentLineDetailFilter Filter)
        {
            List<FulfillmentLineDetail> objAdjustments = null;
            FulfillmentLineDetail objReturn = null;

            try
            {
                objAdjustments = GetFulfillmentLineDetails(CompanyID, Filter);
                if (objAdjustments != null && objAdjustments.Count >= 1) objReturn = objAdjustments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAdjustments = null;
            }

            return objReturn;
        }

        public static List<FulfillmentLineDetail> GetFulfillmentLineDetails(string CompanyID)
        {
            int intTotalCount = 0;
            return GetFulfillmentLineDetails(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<FulfillmentLineDetail> GetFulfillmentLineDetails(string CompanyID, FulfillmentLineDetailFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillmentLineDetails(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<FulfillmentLineDetail> GetFulfillmentLineDetails(string CompanyID, FulfillmentLineDetailFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetFulfillmentLineDetails(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<FulfillmentLineDetail> GetFulfillmentLineDetails(string CompanyID, FulfillmentLineDetailFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<FulfillmentLineDetail> objReturn = null;
            FulfillmentLineDetail objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<FulfillmentLineDetail>();

                strSQL = "SELECT * " +
                         "FROM FulfillmentLineDetail (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.FulfillmentLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FulfillmentLineID, "FulfillmentLineID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "TransferLineDetailID"
                            : Utility.CustomSorting.GetSortExpression(typeof(TransferLineDetail), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new FulfillmentLineDetail(objData.Tables[0].Rows[i]);
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