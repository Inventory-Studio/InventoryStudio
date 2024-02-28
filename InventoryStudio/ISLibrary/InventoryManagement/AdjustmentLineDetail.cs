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
    public class AdjustmentLineDetail : BaseClass
    {
        public string AdjustmentLineDetailID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(AdjustmentLineDetailID); }
        }
        public string CompanyID { get; set; }
        public string AdjustmentLineID { get; set; }
        public string BinID { get; set; }
        public int Quantity { get; set; }
        public int? ItemUnitID { get; set; }
        public decimal BaseQuantity { get; set; }
        public string InventoryID { get; set; }
        //public string CartonNumber { get; set; }
        //public string VendorCartonNumber { get; set; }
        //public string SerialNumber { get; set; }
        //public string LotNumber { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<AdjustmentLineDetail> AdjustmentLineDetails { get; set; }


        public AdjustmentLineDetail()
        {
        }

        public AdjustmentLineDetail( /*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.AdjustmentLineDetailID = AdjustmentLineDetailID;
            this.Load();
        }

        public AdjustmentLineDetail(DataRow objRow)
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
                         "FROM AdjustmentLineDetail (NOLOCK) " +
                         "WHERE AdjustmentLineDetailID=" + Database.HandleQuote(AdjustmentLineDetailID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AdjustmentLineDetailID=" + AdjustmentLineDetailID + " is not found");
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

                if (objColumns.Contains("AdjustmentLineDetailID")) AdjustmentLineDetailID = Convert.ToString(objRow["AdjustmentLineDetailID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("AdjustmentLineID")) AdjustmentLineID = Convert.ToString(objRow["AdjustmentLineID"]);
                if (objColumns.Contains("BinID")) BinID = Convert.ToString(objRow["BinID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt16(objRow["Quantity"]);
                if (objColumns.Contains("BaseQuantity")) BaseQuantity = Convert.ToDecimal(objRow["BaseQuantity"]);
                if (objColumns.Contains("InventoryID")) InventoryID = Convert.ToString(objRow["InventoryID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AdjustmentLineDetailID)) throw new Exception("Missing AdjustmentLineDetailID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, AdjustmentLineDetailID already exists");


                dicParam["CompanyID"] = CompanyID;
                dicParam["AdjustmentLineID"] = AdjustmentLineID;
                dicParam["BinID"] = BinID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["BaseQuantity"] = BaseQuantity;
                dicParam["InventoryID"] = InventoryID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                AdjustmentLineDetailID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AdjustmentLineDetailID"), objConn, objTran)
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


        public static AdjustmentLineDetail GetAdjustmentLineDetail(string CompanyID, AdjustmentLineDetailFilter Filter)
        {
            List<AdjustmentLineDetail> objAdjustments = null;
            AdjustmentLineDetail objReturn = null;

            try
            {
                objAdjustments = GetAdjustmentLineDetails(CompanyID, Filter);
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

        public static List<AdjustmentLineDetail> GetAdjustmentLineDetails(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAdjustmentLineDetails(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AdjustmentLineDetail> GetAdjustmentLineDetails(string CompanyID, AdjustmentLineDetailFilter Filter)
        {
            int intTotalCount = 0;
            return GetAdjustmentLineDetails(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AdjustmentLineDetail> GetAdjustmentLineDetails(string CompanyID, AdjustmentLineDetailFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetAdjustmentLineDetails(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AdjustmentLineDetail> GetAdjustmentLineDetails(string CompanyID, AdjustmentLineDetailFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AdjustmentLineDetail> objReturn = null;
            AdjustmentLineDetail objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AdjustmentLineDetail>();

                strSQL = "SELECT * " +
                         "FROM AdjustmentLineDetail (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.AdjustmentLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AdjustmentLineID, "AdjustmentLineID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "AdjustmentLineDetailID"
                            : Utility.CustomSorting.GetSortExpression(typeof(AdjustmentLineDetail), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AdjustmentLineDetail(objData.Tables[0].Rows[i]);
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