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
    public class FulfillmentPackageLine : BaseClass
    {
        public string FulfillmentPackageLineID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(FulfillmentPackageLineID); }
        }     
        public string CompanyID { get; set; }
        public string FulfillmentPackageID { get; set; }
        public string FulfillmentLineID { get; set; }
        public string ItemID { get; set; }
        public decimal Quantity { get; set; }
        public string ItemUnitID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }



        public FulfillmentPackageLine()
        {
        }

        public FulfillmentPackageLine( string CompanyID, string FulfillmentPackageLineID)
        {
            this.CompanyID = CompanyID;
            this.FulfillmentPackageLineID = FulfillmentPackageLineID;
            this.Load();
        }

        public FulfillmentPackageLine(DataRow objRow)
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
                         "FROM FulfillmentPackageLine (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                        " AND FulfillmentPackageLineID=" + Database.HandleQuote(FulfillmentPackageLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentPackageLineID=" + FulfillmentPackageLineID + " is not found");
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
                if (objColumns.Contains("FulfillmentPackageLineID")) FulfillmentPackageLineID = Convert.ToString(objRow["FulfillmentPackageLineID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("FulfillmentPackageID")) FulfillmentPackageID = Convert.ToString(objRow["FulfillmentPackageID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToDecimal(objRow["Quantity"]);
                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);


                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
           

                if (string.IsNullOrEmpty(FulfillmentPackageLineID)) throw new Exception("Missing FulfillmentPackageLineID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, FulfillmentPackageLineID already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["FulfillmentPackageID"] = FulfillmentPackageID;
                dicParam["FulfillmentLineID"] = FulfillmentLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;

                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                FulfillmentPackageLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "FulfillmentPackageLine"), objConn, objTran)
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


        public static FulfillmentPackageLine GetFulfillmentPackageLine(string CompanyID, FulfillmentPackageLineFilter Filter)
        {
            List<FulfillmentPackageLine> objAdjustments = null;
            FulfillmentPackageLine objReturn = null;

            try
            {
                objAdjustments = GetFulfillmentPackageLines(CompanyID, Filter);
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

        public static List<FulfillmentPackageLine> GetFulfillmentPackageLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetFulfillmentPackageLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<FulfillmentPackageLine> GetFulfillmentPackageLines(string CompanyID, FulfillmentPackageLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillmentPackageLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<FulfillmentPackageLine> GetFulfillmentPackageLines(string CompanyID, FulfillmentPackageLineFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetFulfillmentPackageLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<FulfillmentPackageLine> GetFulfillmentPackageLines(string CompanyID, FulfillmentPackageLineFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<FulfillmentPackageLine> objReturn = null;
            FulfillmentPackageLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<FulfillmentPackageLine>();

                strSQL = "SELECT * " +
                         "FROM FulfillmentPackageLine (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.FulfillmentPackageID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FulfillmentPackageID, "FulfillmentPackageID");
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
                        objNew = new FulfillmentPackageLine(objData.Tables[0].Rows[i]);
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