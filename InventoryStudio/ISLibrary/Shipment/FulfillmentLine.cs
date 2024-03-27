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
    public class FulfillmentLine : BaseClass
    {
        public string FulfillmentLineID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(FulfillmentLineID); }
        }     
        public string CompanyID { get; set; }
        public string FulfillmentID { get; set; }
        public string SalesOrderLineID { get; set; }
        public string ItemID { get; set; }
        public string LocationID { get; set; }
        public decimal Quantity { get; set; }
        public string ItemUnitID { get; set; }
        public bool IsAutoPicked { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<FulfillmentLineDetail> mFulfillmentLineDetail = null;
        public List<FulfillmentLineDetail> FulfillmentLineDetails
        {
            get
            {
                FulfillmentLineDetailFilter objFilter = null;

                try
                {
                    if (mFulfillmentLineDetail == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(FulfillmentLineID))
                    {
                        objFilter = new FulfillmentLineDetailFilter();
                        objFilter.FulfillmentLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.FulfillmentLineID.SearchString = FulfillmentLineID;
                        mFulfillmentLineDetail = FulfillmentLineDetail.GetFulfillmentLineDetails(CompanyID, objFilter);
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
                return mFulfillmentLineDetail;
            }
            set
            {
                mFulfillmentLineDetail = value;
            }
        }



        public FulfillmentLine()
        {
        }

        public FulfillmentLine( string CompanyID, string FulfillmentLineID)
        {
            this.CompanyID = CompanyID;
            this.FulfillmentLineID = FulfillmentLineID;
            this.Load();
        }

        public FulfillmentLine(DataRow objRow)
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
                         "FROM FulfillmentLine (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                        " AND FulfillmentLineID=" + Database.HandleQuote(FulfillmentLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentLineID=" + FulfillmentLineID + " is not found");
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

                if (objColumns.Contains("FulfillmentLineID")) FulfillmentLineID = Convert.ToString(objRow["FulfillmentLineID"]);
                if (objColumns.Contains("FulfillmentID")) FulfillmentID = Convert.ToString(objRow["FulfillmentID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToDecimal(objRow["Quantity"]);
                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("IsAutoPicked")) IsAutoPicked = Convert.ToBoolean(objRow["IsAutoPicked"]);


                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
           

                if (string.IsNullOrEmpty(FulfillmentID)) throw new Exception("Missing FulfillmentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, AdjustmentID already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["FulfillmentID"] = FulfillmentID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["LocationID"] = LocationID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["IsAutoPicked"] = IsAutoPicked;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                FulfillmentLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "FulfillmentLine"), objConn, objTran)
                    .ToString();

                if (FulfillmentLineDetails != null)
                {
                    foreach (FulfillmentLineDetail objFulfillmentLineDetail in FulfillmentLineDetails)
                    {
                        if (objFulfillmentLineDetail.IsNew)
                        {
                            objFulfillmentLineDetail.CompanyID = CompanyID;
                            objFulfillmentLineDetail.FulfillmentLineID = FulfillmentLineID;
                            objFulfillmentLineDetail.CreatedBy = CreatedBy;
                            objFulfillmentLineDetail.ParentKey = FulfillmentID;
                            objFulfillmentLineDetail.ParentObject = "Fulfillment";
                            objFulfillmentLineDetail.Create(objConn, objTran);
                        }
                    }
                }

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


        public static FulfillmentLine GetGetFulfillmentLine(string CompanyID, FulfillmentLineFilter Filter)
        {
            List<FulfillmentLine> objAdjustments = null;
            FulfillmentLine objReturn = null;

            try
            {
                objAdjustments = GetFulfillmentLines(CompanyID, Filter);
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

        public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetFulfillmentLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID, FulfillmentLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillmentLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID, FulfillmentLineFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetFulfillmentLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID, FulfillmentLineFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<FulfillmentLine> objReturn = null;
            FulfillmentLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<FulfillmentLine>();

                strSQL = "SELECT * " +
                         "FROM FulfillmentLine (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.FulfillmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FulfillmentID, "FulfillmentID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "FulfillmentLineID"
                            : Utility.CustomSorting.GetSortExpression(typeof(FulfillmentLine), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new FulfillmentLine(objData.Tables[0].Rows[i]);
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