using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using ISLibrary.OrderManagement;

namespace ISLibrary
{
    public class TransferLine : BaseClass
    {
        public string TransferLineID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(TransferLineID); }
        }
        public string CompanyID { get; set; }
        public string TransferID { get; set; }
        public string ItemID { get; set; }
        public string FromLocationID { get; set; }
        public string ToLocationID { get; set; }
        public decimal Quantity { get; set; }
        public string ItemUnitID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<TransferLineDetail> mTransferLineDetail = null;
        public List<TransferLineDetail> TransferLineDetails
        {
            get
            {
                TransferLineDetailFilter objFilter = null;

                try
                {
                    if (mTransferLineDetail == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(TransferLineID))
                    {
                        objFilter = new TransferLineDetailFilter();
                        objFilter.TransferLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.TransferLineID.SearchString = TransferLineID;
                        mTransferLineDetail = TransferLineDetail.GetTransferLineDetails(CompanyID, objFilter);
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
                return mTransferLineDetail;
            }
            set
            {
                mTransferLineDetail = value;
            }
        }


        private Location mFromLocation = null;
        public Location FromLocation
        {
            get
            {


                try
                {
                    if (mFromLocation == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(FromLocationID))
                    {
                        mFromLocation = new Location(CompanyID, FromLocationID);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return mFromLocation;
            }
            set
            {
                mFromLocation = value;
            }
        }

        private Location mToLocation = null;
        public Location ToLocation
        {
            get
            {


                try
                {
                    if (mToLocation == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ToLocationID))
                    {
                        mToLocation = new Location(CompanyID, ToLocationID);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return mToLocation;
            }
            set
            {
                mToLocation = value;
            }
        }

        private ItemUnit mItemUnit = null;
        public ItemUnit ItemUnit
        {
            get
            {


                try
                {
                    if (mItemUnit == null && !string.IsNullOrEmpty(ItemUnitID))
                    {
                        mItemUnit = new ItemUnit(ItemUnitID);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return mItemUnit;
            }
            set
            {
                mItemUnit = value;
            }
        }
        public TransferLine()
        {
        }

        public TransferLine( /*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.TransferLineID = Id;
            this.Load();
        }

        public TransferLine(DataRow objRow)
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
                         "FROM TransferLine (NOLOCK) " +
                         "WHERE TransferLineID=" + Database.HandleQuote(TransferLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TransferLineID=" + TransferLineID + " is not found");
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

                if (objColumns.Contains("TransferLineID")) TransferLineID = Convert.ToString(objRow["TransferLineID"]);
                if (objColumns.Contains("TransferID")) TransferID = Convert.ToString(objRow["TransferID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("FromLocationID")) FromLocationID = Convert.ToString(objRow["FromLocationID"]);
                if (objColumns.Contains("ToLocationID")) ToLocationID = Convert.ToString(objRow["ToLocationID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt64(objRow["Quantity"]);
                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TransferLineID)) throw new Exception("Missing TransferLineID in the datarow");
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
                dicParam["TransferID"] = TransferID;
                dicParam["ItemID"] = ItemID;
                dicParam["FromLocationID"] = FromLocationID;
                dicParam["ToLocationID"] = ToLocationID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                TransferLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "TransferLine"), objConn, objTran)
                    .ToString();

                //Unit of Measure from Receipt Line
                decimal decQtyPerUnit = 1;
                if (ItemUnitID != null )
                {   
                    decQtyPerUnit = ItemUnit.Quantity;                    
                }

                if (!FromLocation.UseBins)
                {
                    decimal decBaseQuantity = Quantity * decQtyPerUnit;
                    Inventory DefaultInventory = new Inventory();

                    DefaultInventory.CompanyID = CompanyID;
                    DefaultInventory.ItemID = ItemID;
                    DefaultInventory.LocationID = FromLocationID;
                    DefaultInventory.BinID = "3";//@todo set the default value 

                    if (DefaultInventory.GetUnqiueInventory())
                    {
                        DefaultInventory.OnHand -= decBaseQuantity;
                        DefaultInventory.Available -= decBaseQuantity;
                        if (DefaultInventory.OnHand < 0 || DefaultInventory.Available < 0)
                        {
                            throw new Exception("The Quantity of the Item " + ItemID + " < 0 ");
                        }
                        DefaultInventory.Update();
                    }
                    else
                    {
                        throw new Exception("Can't find this item(" + ItemID + ")'s  bin Data.");
                    }
                    
                }

                if (!ToLocation.UseBins)
                {
                    decimal decBaseQuantity = Quantity * decQtyPerUnit;
                    Inventory DefaultInventory = new Inventory();

                    DefaultInventory.CompanyID = CompanyID;
                    DefaultInventory.ItemID = ItemID;
                    DefaultInventory.LocationID = ToLocationID;
                    DefaultInventory.Available = decBaseQuantity;
                    DefaultInventory.BinID = "3";//@todo set the default value 
                    DefaultInventory.OnHand = decBaseQuantity;

                    if (DefaultInventory.GetUnqiueInventory())
                    {
                        DefaultInventory.OnHand += decBaseQuantity;
                        DefaultInventory.Available += decBaseQuantity;
                        DefaultInventory.Update();
                    }
                    else
                    {
                        DefaultInventory.Create();
                    }
                }

                if (TransferLineDetails != null)
                {
                    foreach (TransferLineDetail objTransferLineDetail in TransferLineDetails)
                    {
                        decimal decBaseQuantity = objTransferLineDetail.Quantity * decQtyPerUnit;

                        if (FromLocation.UseBins)
                        {
                            Inventory objInventory = new Inventory();

                            objInventory.CompanyID = CompanyID;
                            objInventory.ItemID = ItemID;
                            objInventory.LocationID = FromLocationID;
                            objInventory.BinID = objTransferLineDetail.FromBinID;
                            objInventory.InventoryNumber = objTransferLineDetail.InventoryNumber;

                            if (objInventory.GetUnqiueInventory())
                            {
                                objInventory.OnHand -= decBaseQuantity;
                                objInventory.Available -= decBaseQuantity;
                                objInventory.UpdatedBy = CreatedBy;
                                objInventory.ParentKey = TransferLineID;
                                objInventory.ParentObject = "TransferLine";
                                if (objInventory.OnHand < 0 || objInventory.Available < 0)
                                {
                                    throw new Exception("The Quantity of the Item " + ItemID + " < 0 ");
                                }
                                objInventory.Update();
                            }
                            else
                            {
                                throw new Exception("Can't find this item(" + ItemID + ")'s  bin Data.");
                            }
                        }

                        if (ToLocation.UseBins)
                        {
                            Inventory objInventory = new Inventory();

                            objInventory.CompanyID = CompanyID;
                            objInventory.ItemID = ItemID;
                            objInventory.LocationID = ToLocationID;
                            objInventory.BinID = objTransferLineDetail.FromBinID;
                            objInventory.InventoryNumber = objTransferLineDetail.InventoryNumber;
                            objInventory.ParentKey = TransferLineID;
                            objInventory.ParentObject = "TransferLine";

                            if (objInventory.GetUnqiueInventory())
                            {
                                objInventory.OnHand += decBaseQuantity;
                                objInventory.Available += decBaseQuantity;
                                objInventory.UpdatedBy = CreatedBy;                       
                                objInventory.Update();
                            }
                            else
                            {
                                objInventory.CreatedBy = CreatedBy;
                                objInventory.Create();
                            }
                        }


                        if (objTransferLineDetail.IsNew)
                        {
                            objTransferLineDetail.CompanyID = CompanyID;
                            objTransferLineDetail.TransferLineID = TransferLineID;
                            objTransferLineDetail.CreatedBy = CreatedBy;
                            objTransferLineDetail.ParentKey = TransferLineID;
                            objTransferLineDetail.ParentObject = "TransferLine";
                            objTransferLineDetail.Create(objConn, objTran);
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


        public static TransferLine GetTransferLine(string CompanyID, TransferLineFilter Filter)
        {
            List<TransferLine> objAdjustments = null;
            TransferLine objReturn = null;

            try
            {
                objAdjustments = GetTransferLines(CompanyID, Filter);
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

        public static List<TransferLine> GetTransferLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetTransferLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<TransferLine> GetTransferLines(string CompanyID, TransferLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetTransferLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<TransferLine> GetTransferLines(string CompanyID, TransferLineFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetTransferLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<TransferLine> GetTransferLines(string CompanyID, TransferLineFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<TransferLine> objReturn = null;
            TransferLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<TransferLine>();

                strSQL = "SELECT * " +
                         "FROM TransferLine (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.TransferID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TransferID, "TransferID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "TransferLineID"
                            : Utility.CustomSorting.GetSortExpression(typeof(TransferLine), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new TransferLine(objData.Tables[0].Rows[i]);
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