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
    public class TransferLineDetail : BaseClass
    {
        public string TransferLineDetailID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(TransferLineDetailID); }
        }
        public string CompanyID { get; set; }
        public string TransferLineID { get; set; }
        public string FromBinID { get; set; }
        public string ToBinID { get; set; }
        public int Quantity { get; set; }
        public int? ItemUnitID { get; set; }
        public decimal BaseQuantity { get; set; }   
        public string InventoryNumber { get; set; }
        public string Type { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

 
        public TransferLineDetail()
        {
        }

        public TransferLineDetail( /*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.TransferLineDetailID = TransferLineDetailID;
            this.Load();
        }

        public TransferLineDetail(DataRow objRow)
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
                         "FROM TransferLineDetail (NOLOCK) " +
                         "WHERE TransferLineDetailID=" + Database.HandleQuote(TransferLineDetailID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TransferLineDetailID=" + TransferLineDetailID + " is not found");
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

                if (objColumns.Contains("TransferLineDetailID")) TransferLineDetailID = Convert.ToString(objRow["TransferLineDetailID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("TransferLineID")) TransferLineID = Convert.ToString(objRow["TransferLineID"]);
                if (objColumns.Contains("FromBinID")) FromBinID = Convert.ToString(objRow["FromBinID"]);
                if (objColumns.Contains("ToBinID")) ToBinID = Convert.ToString(objRow["ToBinID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt16(objRow["Quantity"]);
                if (objColumns.Contains("BaseQuantity")) BaseQuantity = Convert.ToDecimal(objRow["BaseQuantity"]);
                if (objColumns.Contains("InventoryNumber")) InventoryNumber = Convert.ToString(objRow["InventoryNumber"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TransferLineDetailID)) throw new Exception("Missing TransferLineDetailID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, TransferLineDetailID already exists");


                dicParam["CompanyID"] = CompanyID;
                dicParam["TransferLineID"] = TransferLineID;
                dicParam["FromBinID"] = FromBinID;
                dicParam["ToBinID"] = ToBinID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["BaseQuantity"] = BaseQuantity;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                TransferLineDetailID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "TransferLineDetail"), objConn, objTran)
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


        public static TransferLineDetail GetTransferLineDetail(string CompanyID, TransferLineDetailFilter Filter)
        {
            List<TransferLineDetail> objAdjustments = null;
            TransferLineDetail objReturn = null;

            try
            {
                objAdjustments = GetTransferLineDetails(CompanyID, Filter);
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

        public static List<TransferLineDetail> GetTransferLineDetails(string CompanyID)
        {
            int intTotalCount = 0;
            return GetTransferLineDetails(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<TransferLineDetail> GetTransferLineDetails(string CompanyID, TransferLineDetailFilter Filter)
        {
            int intTotalCount = 0;
            return GetTransferLineDetails(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<TransferLineDetail> GetTransferLineDetails(string CompanyID, TransferLineDetailFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetTransferLineDetails(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<TransferLineDetail> GetTransferLineDetails(string CompanyID, TransferLineDetailFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<TransferLineDetail> objReturn = null;
            TransferLineDetail objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<TransferLineDetail>();

                strSQL = "SELECT * " +
                         "FROM TransferLineDetail (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.TransferLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TransferLineID, "TransferLineID");
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
                        objNew = new TransferLineDetail(objData.Tables[0].Rows[i]);
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