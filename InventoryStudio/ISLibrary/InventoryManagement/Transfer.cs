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
    public class Transfer : BaseClass
    {
        public string TransferID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(TransferID); }
        }        
        public string CompanyID { get; set; }
        public DateTime? TranDate { get; set; }
        public string Memo { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        private List<TransferLine> mTransferLine = null;
        public List<TransferLine> TransferLines
        {
            get
            {
                TransferLineFilter objFilter = null;

                try
                {
                    if (mTransferLine == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(TransferID))
                    {
                        objFilter = new TransferLineFilter();
                        objFilter.TransferID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.TransferID.SearchString = TransferID;
                        mTransferLine = TransferLine.GetTransferLines(CompanyID, objFilter);
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
                return mTransferLine;
            }
            set
            {
                mTransferLine = value;
            }
        }
    
        public Transfer()
        {
        }

        public Transfer( /*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.TransferID = Id;
            this.Load();
        }

        public Transfer(DataRow objRow)
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
                         "FROM Transfer (NOLOCK) " +
                         "WHERE TransferID=" + Database.HandleQuote(TransferID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TransferID=" + TransferID + " is not found");
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

                if (objColumns.Contains("TransferID")) TransferID = Convert.ToString(objRow["TransferID"]);
                if (objColumns.Contains("Memo")) Memo = Convert.ToString(objRow["Memo"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("TranDate")) TranDate = Convert.ToDateTime(objRow["TranDate"]);

                if (string.IsNullOrEmpty(TransferID)) throw new Exception("Missing TransferID in the datarow");
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
                dicParam["TranDate"] = DateTime.UtcNow; 
                dicParam["Memo"] = Memo;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                TransferID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Transfer"), objConn, objTran)
                    .ToString();

                if (TransferLines != null)
                {
                    foreach (TransferLine objTransferLine in TransferLines)
                    {
                        if (objTransferLine.IsNew)
                        {
                            objTransferLine.CompanyID = CompanyID;
                            objTransferLine.TransferID = TransferID;
                            objTransferLine.CreatedBy = CreatedBy;
                            objTransferLine.ParentKey = TransferID;
                            objTransferLine.ParentObject = "Transfer";
                            objTransferLine.Create(objConn, objTran);
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


        public static Transfer GetTransfer(string CompanyID, TransferFilter Filter)
        {
            List<Transfer> objAdjustments = null;
            Transfer objReturn = null;

            try
            {
                objAdjustments = GetTransfers(CompanyID, Filter);
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

        public static List<Transfer> GetTransfers(string CompanyID)
        {
            int intTotalCount = 0;
            return GetTransfers(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Transfer> GetTransfers(string CompanyID, TransferFilter Filter)
        {
            int intTotalCount = 0;
            return GetTransfers(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Transfer> GetTransfers(string CompanyID, TransferFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetTransfers(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Transfer> GetTransfers(string CompanyID, TransferFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Transfer> objReturn = null;
            Transfer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Transfer>();

                strSQL = "SELECT * " +
                         "FROM Transfer (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.TranDate != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TranDate, "TranDate");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "TransferID"
                            : Utility.CustomSorting.GetSortExpression(typeof(Transfer), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Transfer(objData.Tables[0].Rows[i]);
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