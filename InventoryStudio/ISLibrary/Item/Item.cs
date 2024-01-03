using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CLRFramework;

namespace ISLibrary
{
    public class Item : BaseClass
    {
        //public enum enumItemType
        //{
        //    [Description("Inventory Item")]
        //    Inventory,
        //    [Description("Lot Numbered Inventory Item")]
        //    LotNumbered,
        //    [Description("Serialized Inventory Item")]
        //    Serialized,
        //    [Description("Non-Inventory Item")]
        //    NonInventory,
        //    [Description("Kit Item")]
        //    Kit
        //}

        public enum enumWeightUnit
        {
            [Description("_lb")]
            _lb,
            [Description("_oz")]
            _oz,
            [Description("_kg")]
            _kg,
            [Description("_g")]
            _g
        }

        public string ItemID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemID); } }
        public string CompanyID { get; set; }
        public string ClientID { get; set; }
        public string VendorID { get; set; }
        public string ItemParentID { get; set; }
        public string ItemType { get; set; }
        //public enumItemType? ItemType { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public string SalesDescription { get; set; }
        public string PurchaseDescription { get; set; }
        public string Barcode { get; set; }
        public bool IsBarcoded { get; set; }
        public bool IsShipReceiveIndividually { get; set; }
        public bool DisplayComponents { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal? PackageWeight { get; set; }
        public string PackageWeightUOM { get; set; }
        public decimal? PackageLength { get; set; }
        public decimal? PackageWidth { get; set; }
        public decimal? PackageHeight { get; set; }
        public string PackageDimensionUOM { get; set; }
        public string Memo { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private List<ItemAttributeValueLine> mItemAttributeValueLines = null;
        public List<ItemAttributeValueLine> ItemAttributeValueLines
        {
            get
            {
                ItemAttributeValueLineFilter objFilter = null;

                try
                {
                    if (mItemAttributeValueLines == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemID))
                    {
                        objFilter = new ItemAttributeValueLineFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemAttributeValueLines = ItemAttributeValueLine.GetItemAttributeValueLines(CompanyID, objFilter);
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
                return mItemAttributeValueLines;
            }
            set
            {
                mItemAttributeValueLines = value;
            }
        }

        private List<ItemKit> mItemKits = null;
        public List<ItemKit> ItemKits
        {
            get
            {
                ItemKitFilter objFilter = null;

                try
                {
                    if (mItemKits == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemID))
                    {
                        objFilter = new ItemKitFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemKits = ItemKit.GetItemKits(CompanyID, objFilter);
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
                return mItemKits;
            }
            set
            {
                mItemKits = value;
            }
        }

        private List<ItemComponent> mItemComponents = null;
        public List<ItemComponent> ItemComponents
        {
            get
            {
                ItemComponentFilter objFilter = null;

                try
                {
                    if (mItemComponents == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemID))
                    {
                        objFilter = new ItemComponentFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemComponents = ItemComponent.GetItemComponents(CompanyID, objFilter);
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
                return mItemComponents;
            }
            set
            {
                mItemComponents = value;
            }
        }

        private List<ItemBarcode> mItemBarcodes = null;
        public List<ItemBarcode> ItemBarcodes
        {
            get
            {
                ItemBarcodeFilter objFilter = null;

                try
                {
                    if (mItemBarcodes == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemID))
                    {
                        objFilter = new ItemBarcodeFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemBarcodes = ItemBarcode.GetItemBarcodes(CompanyID, objFilter);
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
                return mItemBarcodes;
            }
            set
            {
                mItemBarcodes = value;
            }
        }
        public bool IsVariation
        {
            get
            {
                return ItemAttributeValueLines != null && ItemAttributeValueLines.Count > 0;
            }
        }

        public Item()
        {
        }

        public Item(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public Item(string CompanyID, string ItemID)
        {
            this.CompanyID = CompanyID;
            this.ItemID = ItemID;
            Load();
        }

        public Item(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT i.* " +
                         "FROM Item i (NOLOCK) " +
                         "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND i.ItemID = " + Database.HandleQuote(ItemID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Item is not found");
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
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ClientID")) ClientID = Convert.ToString(objRow["ClientID"]);
                if (objColumns.Contains("ItemParentID")) ItemParentID = Convert.ToString(objRow["ItemParentID"]);
                //if (objColumns.Contains("ItemType") && objRow["ItemType"] != DBNull.Value) Enum.TryParse(Convert.ToString(objRow["ItemType"]), out enumItemType ItemType);
                if (objColumns.Contains("ItemType")) ItemType = Convert.ToString(objRow["ItemType"]);
                if (objColumns.Contains("ItemNumber")) ItemNumber = Convert.ToString(objRow["ItemNumber"]);
                if (objColumns.Contains("ItemName")) ItemName = Convert.ToString(objRow["ItemName"]);
                if (objColumns.Contains("SalesDescription")) SalesDescription = Convert.ToString(objRow["SalesDescription"]);
                if (objColumns.Contains("PurchaseDescription")) PurchaseDescription = Convert.ToString(objRow["PurchaseDescription"]);
                if (objColumns.Contains("Barcode")) Barcode = Convert.ToString(objRow["Barcode"]);
                if (objColumns.Contains("IsBarcoded")) IsBarcoded = Convert.ToBoolean(objRow["IsBarcoded"]);
                if (objColumns.Contains("IsShipReceiveIndividually")) IsShipReceiveIndividually = Convert.ToBoolean(objRow["IsShipReceiveIndividually"]);
                if (objColumns.Contains("DisplayComponents")) DisplayComponents = Convert.ToBoolean(objRow["DisplayComponents"]);
                if (objColumns.Contains("UnitOfMeasure")) UnitOfMeasure = Convert.ToString(objRow["UnitOfMeasure"]);
                if (objColumns.Contains("Memo")) Memo = Convert.ToString(objRow["Memo"]);
                if (objColumns.Contains("PackageWeight") && objRow["PackageWeight"] != DBNull.Value) PackageWeight = Convert.ToDecimal(objRow["PackageWeight"]);
                if (objColumns.Contains("PackageWeightUOM")) PackageWeightUOM = Convert.ToString(objRow["PackageWeightUOM"]);
                if (objColumns.Contains("PackageLength") && objRow["PackageLength"] != DBNull.Value) PackageLength = Convert.ToDecimal(objRow["PackageLength"]);
                if (objColumns.Contains("PackageWidth") && objRow["PackageWidth"] != DBNull.Value) PackageWidth = Convert.ToDecimal(objRow["PackageWidth"]);
                if (objColumns.Contains("PackageHeight") && objRow["PackageHeight"] != DBNull.Value) PackageHeight = Convert.ToDecimal(objRow["PackageHeight"]);
                if (objColumns.Contains("PackageDimensionUOM")) PackageDimensionUOM = Convert.ToString(objRow["PackageDimensionUOM"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemID)) throw new Exception("Missing ItemID in the datarow");
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
            ItemParent objItemParent = null;

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ItemParentID)) throw new Exception("ItemParentID is required");
                if (ItemType == null) throw new Exception("ItemType is required");
                if (string.IsNullOrEmpty(ItemNumber)) throw new Exception("ItemNumber is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemNumber already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["ItemParentID"] = ItemParentID;
                dicParam["ItemType"] = ItemType;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["ItemName"] = ItemName;
                dicParam["SalesDescription"] = SalesDescription;
                dicParam["PurchaseDescription"] = PurchaseDescription;
                dicParam["Barcode"] = Barcode;
                dicParam["IsBarcoded"] = IsBarcoded;
                dicParam["IsShipReceiveIndividually"] = IsShipReceiveIndividually;
                dicParam["DisplayComponents"] = DisplayComponents;
                dicParam["UnitOfMeasure"] = UnitOfMeasure;
                dicParam["PackageWeight"] = PackageWeight;
                dicParam["PackageWeightUOM"] = PackageWeightUOM;
                dicParam["PackageLength"] = PackageLength;
                dicParam["PackageWidth"] = PackageWidth;
                dicParam["PackageHeight"] = PackageHeight;
                dicParam["PackageDimensionUOM"] = PackageDimensionUOM;

                dicParam["CreatedBy"] = CreatedBy;
                ItemID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Item"), objConn, objTran).ToString();

                if (ItemAttributeValueLines != null)
                {

                    foreach (ItemAttributeValueLine objItemAttributeValueLine in ItemAttributeValueLines)
                    {
                        if (objItemAttributeValueLine.IsNew)
                        {
                            //objItemAttributeValueLine.IsLoaded = false;
                            objItemAttributeValueLine.CompanyID = CompanyID;
                            objItemAttributeValueLine.ItemID = ItemID;
                            objItemAttributeValueLine.CreatedBy = CreatedBy;
                            objItemAttributeValueLine.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemAttributeValueLine.UpdatedBy = CreatedBy;
                            objItemAttributeValueLine.Update(objConn, objTran);
                        }
                    }
                }

                if (ItemKits != null)
                {
                    foreach (ItemKit objItemKit in ItemKits)
                    {
                        if (objItemKit.IsNew)
                        {
                            //objItemKit.IsLoaded = false;
                            objItemKit.CompanyID = CompanyID;
                            objItemKit.ItemID = ItemID;
                            objItemKit.CreatedBy = CreatedBy;
                            objItemKit.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemKit.UpdatedBy = CreatedBy;
                            objItemKit.Update(objConn, objTran);
                        }
                    }
                }

                if (ItemBarcodes != null)
                {
                    foreach (ItemBarcode objItemBarcode in ItemBarcodes)
                    {
                        if (objItemBarcode.IsNew)
                        {
                            //objItemBarcode.IsLoaded = false;
                            objItemBarcode.CompanyID = CompanyID;
                            objItemBarcode.ItemID = ItemID;
                            objItemBarcode.CreatedBy = CreatedBy;
                            objItemBarcode.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemBarcode.UpdatedBy = CreatedBy;
                            objItemBarcode.Update(objConn, objTran);
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
                objItemParent = null;
            }
            return true;
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
            ItemParent objItemParent = null;

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ItemParentID)) throw new Exception("ItemParentID is required");
                if (ItemType == null) throw new Exception("ItemType is required");
                if (string.IsNullOrEmpty(ItemNumber)) throw new Exception("ItemNumber is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["ItemParentID"] = ItemParentID;
                dicParam["ItemType"] = ItemType;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["ItemName"] = ItemName;
                dicParam["SalesDescription"] = SalesDescription;
                dicParam["PurchaseDescription"] = PurchaseDescription;
                dicParam["Barcode"] = Barcode;
                dicParam["IsBarcoded"] = IsBarcoded;
                dicParam["IsShipReceiveIndividually"] = IsShipReceiveIndividually;
                dicParam["DisplayComponents"] = DisplayComponents;
                dicParam["UnitOfMeasure"] = UnitOfMeasure;
                dicParam["Memo"] = Memo;
                dicParam["PackageWeight"] = PackageWeight;
                dicParam["PackageWeightUOM"] = PackageWeightUOM;
                dicParam["PackageLength"] = PackageLength;
                dicParam["PackageWidth"] = PackageWidth;
                dicParam["PackageHeight"] = PackageHeight;
                dicParam["PackageDimensionUOM"] = PackageDimensionUOM;

                dicParam["UpdatedBy"] = UpdatedBy;
                dicWParam["ItemID"] = ItemID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Item"), objConn, objTran);

                Item currentItem = new Item(CompanyID, ItemID);

                //Delete no longer existing item attributevalueline for the item
                foreach (ItemAttributeValueLine _currentItemAttributeValueLine in currentItem.ItemAttributeValueLines)
                {
                    if (!ItemAttributeValueLines.Exists(x => x.ItemAttributeValueLineID == _currentItemAttributeValueLine.ItemAttributeValueLineID))
                    {
                        //_currentItemAttributeValueLine.IsLoaded = true;
                        _currentItemAttributeValueLine.Delete(objConn, objTran);
                    }
                }

                if (ItemAttributeValueLines != null)
                {
                    foreach (ItemAttributeValueLine objItemAttributeValueLine in ItemAttributeValueLines)
                    {
                        if (objItemAttributeValueLine.IsNew)
                        {
                            objItemAttributeValueLine.ItemID = ItemID;
                            objItemAttributeValueLine.CreatedBy = UpdatedBy;
                            objItemAttributeValueLine.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemAttributeValueLine.ItemID = ItemID;
                            objItemAttributeValueLine.UpdatedBy = UpdatedBy;
                            objItemAttributeValueLine.Update(objConn, objTran);
                        }
                    }
                }

                //Delete no longer existing item attributevalueline for the item
                foreach (ItemKit _currentItemKit in currentItem.ItemKits)
                {
                    if (!ItemKits.Exists(x => x.ItemKitID == _currentItemKit.ItemKitID))
                    {
                        _currentItemKit.Delete(objConn, objTran);
                    }
                }

                if (ItemKits != null)
                {
                    foreach (ItemKit objItemKit in ItemKits)
                    {
                        if (objItemKit.IsNew)
                        {
                            objItemKit.CompanyID = CompanyID;
                            objItemKit.ItemID = ItemID;
                            objItemKit.CreatedBy = CreatedBy;
                            objItemKit.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemKit.UpdatedBy = UpdatedBy;
                            objItemKit.Update(objConn, objTran);
                        }
                    }
                }

                foreach (ItemBarcode _currentItemBarcode in currentItem.ItemBarcodes)
                {
                    if (!ItemBarcodes.Exists(x => x.ItemBarcodeID == _currentItemBarcode.ItemBarcodeID))
                    {
                        _currentItemBarcode.Delete(objConn, objTran);
                    }
                }

                if (ItemBarcodes != null)
                {
                    foreach (ItemBarcode objItemBarcode in ItemBarcodes)
                    {
                        if (objItemBarcode.IsNew)
                        {
                            objItemBarcode.CompanyID = CompanyID;
                            objItemBarcode.ItemID = ItemID;
                            objItemBarcode.CreatedBy = CreatedBy;
                            objItemBarcode.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemBarcode.UpdatedBy = UpdatedBy;
                            objItemBarcode.Update(objConn, objTran);
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
                dicWParam = null;
            }
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemID is missing");

                foreach (ItemAttributeValueLine objItemAttributeValueLine in ItemAttributeValueLines)
                {
                    objItemAttributeValueLine.Delete(objConn, objTran);
                }
                
                dicDParam["ItemID"] = ItemID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Item"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM Item (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemNumber=" + Database.HandleQuote(ItemNumber);
            if (!string.IsNullOrEmpty(ClientID)) strSQL += "AND p.ClientID=" + Database.HandleQuote(ClientID);

            if (!string.IsNullOrEmpty(ItemID)) strSQL += "AND p.ItemID<>" + Database.HandleQuote(ItemID);
            return Database.HasRows(strSQL);
        }

        public static Item GetItem(string CompanyID, ItemFilter Filter)
        {
            List<Item> objItems = null;
            Item objReturn = null;

            try
            {
                objItems = GetItems(CompanyID, Filter);
                if (objItems != null && objItems.Count >= 1) objReturn = objItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItems = null;
            }
            return objReturn;
        }

        public static List<Item> GetItems(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItems(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Item> GetItems(string CompanyID, ItemFilter Filter)
        {
            int intTotalCount = 0;
            return GetItems(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Item> GetItems(string CompanyID, ItemFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItems(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Item> GetItems(string CompanyID, ItemFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Item> objReturn = null;
            Item objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Item>();

                strSQL = "SELECT f.* " +
                         "FROM Item (NOLOCK) f " +
                         "WHERE f.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.ClientID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ClientID, "f.ClientID");
                    if (Filter.ItemParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemParentID, "f.ItemParentID");
                    if (Filter.ItemTypes != null && Filter.ItemTypes.Count > 0) strSQL += " AND ItemType IN (" + String.Join(",", Filter.ItemTypes.Select(m => Database.HandleQuote(m.ToString())).ToArray()) + ") ";
                    if (Filter.ItemNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemNumber, "f.ItemNumber");
                    if (Filter.ItemName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemName, "f.ItemName");
                    if (Filter.SalesDescription != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesDescription, "f.SalesDescription");
                    if (Filter.PurchaseDescription != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseDescription, "f.PurchaseDescription");
                    if (Filter.Barcode != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Barcode, "f.Barcode");

                    if (Filter.IsBarcoded != null) strSQL += "AND f.IsBarcoded=" + Database.HandleQuote(Convert.ToInt32(Filter.IsBarcoded.Value).ToString());
                    if (Filter.IsShipReceiveIndividually != null) strSQL += "AND f.IsShipReceiveIndividually=" + Database.HandleQuote(Convert.ToInt32(Filter.IsShipReceiveIndividually.Value).ToString());
                    if (Filter.DisplayComponents != null) strSQL += "AND f.DisplayComponents=" + Database.HandleQuote(Convert.ToInt32(Filter.DisplayComponents.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemID" : Utility.CustomSorting.GetSortExpression(typeof(Item), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Item(objData.Tables[0].Rows[i]);
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
