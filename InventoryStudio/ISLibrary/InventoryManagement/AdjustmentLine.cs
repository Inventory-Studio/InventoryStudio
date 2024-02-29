using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using ISLibrary.OrderManagement;

namespace ISLibrary
{
    public class AdjustmentLine : BaseClass
    {
        public string AdjustmentLineID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(AdjustmentLineID); }
        }
        public string CompanyID { get; set; }
        public string AdjustmentID { get; set; }
        public string ItemID { get; set; }
        public string LocationID { get; set; }
        public decimal Quantity { get; set; }
        public string ItemUnitID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<AdjustmentLineDetail> AdjustmentLineDetails { get; set; }


        private Location mLocation = null;
        public Location Location
        {
            get
            {
               

                try
                {
                    if (mLocation == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(LocationID))
                    {
                        mLocation = new Location(CompanyID,LocationID);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
               
                return mLocation;
            }
            set
            {
                mLocation = value;
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
        public AdjustmentLine()
        {
        }

        public AdjustmentLine( /*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.AdjustmentLineID = Id;
            this.Load();
        }

        public AdjustmentLine(DataRow objRow)
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
                         "FROM AdjustmentLine (NOLOCK) " +
                         "WHERE AdjustmentLineID=" + Database.HandleQuote(AdjustmentLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AdjustmentLinetID=" + AdjustmentLineID + " is not found");
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

                if (objColumns.Contains("AdjustmentLineID")) AdjustmentLineID = Convert.ToString(objRow["AdjustmentLineID"]);
                if (objColumns.Contains("AdjustmentID")) AdjustmentID = Convert.ToString(objRow["AdjustmentID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt64(objRow["Quantity"]);
                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AdjustmentID)) throw new Exception("Missing AdjustmentID in the datarow");
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
                dicParam["AdjustmentID"] = AdjustmentID;
                dicParam["ItemID"] = ItemID;
                dicParam["LocationID"] = LocationID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemUnitID"] = ItemUnitID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                AdjustmentLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AdjustmentLine"), objConn, objTran)
                    .ToString();

                //Unit of Measure from Receipt Line
                decimal decQtyPerUnit = 1;
                if (ItemUnitID != null && ItemUnit != null)
                {   
                    decQtyPerUnit = ItemUnit.Quantity;                    
                }

                if (!Location.UseBins)
                {
                    decimal decBaseQuantity = Quantity * decQtyPerUnit;
                    Inventory DefaultInventory = new Inventory();

                    DefaultInventory.CompanyID = CompanyID;
                    DefaultInventory.ItemID = ItemID;
                    DefaultInventory.LocationID = LocationID;
                    DefaultInventory.Available = Quantity;
                    DefaultInventory.BinID = "3";//@todo set the default value 
                    DefaultInventory.OnHand = Quantity;
                    
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



                if (AdjustmentLineDetails != null)
                {
                    foreach (AdjustmentLineDetail objAdjustmentLineDetail in AdjustmentLineDetails)
                    {
                        decimal decBaseQuantity = objAdjustmentLineDetail.Quantity * decQtyPerUnit;
                        
                        Inventory objInventory = new Inventory();

                        objInventory.CompanyID = objAdjustmentLineDetail.CompanyID;
                        objInventory.ItemID = ItemID;
                        objInventory.LocationID = LocationID;
                        objInventory.Available = decBaseQuantity;
                        objInventory.OnHand = decBaseQuantity;
                        objInventory.BinID = objAdjustmentLineDetail.BinID;
                        objInventory.InventoryNumber = objAdjustmentLineDetail.InventoryNumber;

                        if (objInventory.GetUnqiueInventory())
                        {
                            objInventory.OnHand += decBaseQuantity;
                            objInventory.Available += decBaseQuantity;
                            objInventory.UpdatedBy = CreatedBy;
                            objInventory.ParentKey = AdjustmentLineID;
                            objInventory.ParentObject = "AdjustmentLine";
                            objInventory.Update();
                        }
                        else
                        {
                            objInventory.ParentKey = AdjustmentLineID;
                            objInventory.ParentObject = "AdjustmentLine";
                            objInventory.CreatedBy = CreatedBy;
                            objInventory.Create();
                        }

                        if (objAdjustmentLineDetail.IsNew)
                        {
                            objAdjustmentLineDetail.CompanyID = CompanyID;
                            objAdjustmentLineDetail.AdjustmentLineID = AdjustmentLineID;
                            objAdjustmentLineDetail.CreatedBy = CreatedBy;
                            objAdjustmentLineDetail.InventoryID = objInventory.InventoryID;
                            objAdjustmentLineDetail.ParentKey = AdjustmentLineID;
                            objAdjustmentLineDetail.ParentObject = "AdjustmentLine";
                            objAdjustmentLineDetail.Create(objConn, objTran);
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


        public static AdjustmentLine GetAdjustmentLine(string CompanyID, AdjustmentLineFilter Filter)
        {
            List<AdjustmentLine> objAdjustments = null;
            AdjustmentLine objReturn = null;

            try
            {
                objAdjustments = GetAdjustmentLines(CompanyID, Filter);
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

        public static List<AdjustmentLine> GetAdjustmentLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAdjustmentLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AdjustmentLine> GetAdjustmentLines(string CompanyID, AdjustmentLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetAdjustmentLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AdjustmentLine> GetAdjustmentLines(string CompanyID, AdjustmentLineFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetAdjustmentLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AdjustmentLine> GetAdjustmentLines(string CompanyID, AdjustmentLineFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AdjustmentLine> objReturn = null;
            AdjustmentLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AdjustmentLine>();

                strSQL = "SELECT * " +
                         "FROM AdjustmentLine (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.AdjustmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AdjustmentID, "AdjustmentID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "AdjustmentLineID"
                            : Utility.CustomSorting.GetSortExpression(typeof(AdjustmentLine), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AdjustmentLine(objData.Tables[0].Rows[i]);
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