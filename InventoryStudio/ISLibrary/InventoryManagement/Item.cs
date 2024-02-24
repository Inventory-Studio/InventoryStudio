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
        public enum enumItemType
        {
            [Description("Inventory Item")]
            Inventory,
            [Description("Lot Numbered Inventory Item")]
            LotNumbered,
            [Description("Serialized Inventory Item")]
            Serialized,
            [Description("Assembly Item")]
            Assembly,
            [Description("Kit Item")]
            Kit
        }

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
        public string VendorID { get; set; }
        public string ItemParentID { get; set; }
        //public string ItemType { get; set; }
        [DisplayName("Item Type")]
        public enumItemType? ItemType { get; set; }
        [DisplayName("Item Number")]
        public string ItemNumber { get; set; }
        [DisplayName("Item Name")]
        public string ItemName { get; set; }
        [DisplayName("Sales Description")]
        public string SalesDescription { get; set; }
        [DisplayName("Purchase Description")]
        public string PurchaseDescription { get; set; }
        [DisplayName("Barcode")]
        public string Barcode { get; set; }
        public bool IsBarcoded { get; set; }
        [DisplayName("Receive Individually")]
        public bool IsShipReceiveIndividually { get; set; }
        public bool DisplayComponents { get; set; }
        [DisplayName("Unit of Measure")]
        public string UnitOfMeasure { get; set; }
        [DisplayName("Package Weight")]
        public decimal? PackageWeight { get; set; }
        [DisplayName("Package Weight Unit of Measure")]
        public string PackageWeightUOM { get; set; }
        [DisplayName("Package Length")]
        public decimal? PackageLength { get; set; }
        [DisplayName("Package Width")]
        public decimal? PackageWidth { get; set; }
        [DisplayName("Package Height")]
        public decimal? PackageHeight { get; set; }
        [DisplayName("Package Dimension Unit of Measure")]
        public string PackageDimensionUOM { get; set; }
        public string Memo { get; set; }
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; private set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; private set; }

        //view attribute
        public string ItemParentNumber { get; set; }
        public string Label { get; set; }
        public int? ItemUnitTypeID { get; set; }
        public int? PrimarySalesUnitID { get; set; }
        public int? PrimaryPurchaseUnitID { get; set; }
        public int? PrimaryStockUnitID { get; set; }
        public string ImageURL { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool? UseSingleBin { get; set; }
        [DisplayName("Fulfill By Kit")]
        public bool FulfillByKit { get; set; }
        [DisplayName("Receive By Kit")]
        public bool ReceiveByKit { get; set; }
        [DisplayName("HS Code")]
        public string HSCode { get; set; }
        [DisplayName("Good Description")]
        public string GoodDescription { get; set; }
        [DisplayName("Country of Origin")]
        public string CountryOfOrigin { get; set; }
        public int? BinID { get; set; }

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
        { get; set; }

        public Item()
        {
        }

        public Item(string ItemID)
        {
            this.ItemID = ItemID;
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
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT i.* " +
                         "FROM Item i (NOLOCK) " +
                         "WHERE i.ItemID=" + Database.HandleQuote(ItemID) +
                         "AND i.CompanyID = " + Database.HandleQuote(CompanyID);

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
               
                if (objColumns.Contains("ItemParentID")) ItemParentID = Convert.ToString(objRow["ItemParentID"]);
                if (objColumns.Contains("ItemType") && objRow["ItemType"] != DBNull.Value && Enum.TryParse(Convert.ToString(objRow["ItemType"]), out enumItemType itemType)) ItemType = itemType;
                //if (objColumns.Contains("ItemType")) ItemType = Convert.ToString(objRow["ItemType"]);
                if (objColumns.Contains("ItemNumber")) ItemNumber = Convert.ToString(objRow["ItemNumber"]);
                if (objColumns.Contains("ItemName")) ItemName = Convert.ToString(objRow["ItemName"]);
                if (objColumns.Contains("SalesDescription")) SalesDescription = Convert.ToString(objRow["SalesDescription"]);
                if (objColumns.Contains("PurchaseDescription")) PurchaseDescription = Convert.ToString(objRow["PurchaseDescription"]);
                if (objColumns.Contains("Barcode")) Barcode = Convert.ToString(objRow["Barcode"]);
                if (objColumns.Contains("IsBarcoded")) IsBarcoded = Convert.ToBoolean(objRow["IsBarcoded"]);
                if (objColumns.Contains("IsShipReceiveIndividually")) IsShipReceiveIndividually = Convert.ToBoolean(objRow["IsShipReceiveIndividually"]);
                if (objColumns.Contains("FulfillByKit")) FulfillByKit = Convert.ToBoolean(objRow["FulfillByKit"]);
                if (objColumns.Contains("ReceiveByKit")) ReceiveByKit = Convert.ToBoolean(objRow["ReceiveByKit"]);
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

                if (objColumns.Contains("HSCode")) HSCode = Convert.ToString(objRow["HSCode"]);
                if (objColumns.Contains("GoodDescription")) GoodDescription = Convert.ToString(objRow["GoodDescription"]);
                if (objColumns.Contains("CountryOfOrigin")) CountryOfOrigin = Convert.ToString(objRow["CountryOfOrigin"]);


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
                dicParam["ItemParentID"] = ItemParentID;
                dicParam["ItemType"] = ItemType;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["ItemName"] = ItemName;
                dicParam["SalesDescription"] = SalesDescription;
                dicParam["PurchaseDescription"] = PurchaseDescription;
                dicParam["Barcode"] = Barcode;
                dicParam["IsBarcoded"] = IsBarcoded;
                dicParam["IsShipReceiveIndividually"] = IsShipReceiveIndividually;
                dicParam["FulfillByKit"] = FulfillByKit;
                dicParam["ReceiveByKit"] = ReceiveByKit;
                dicParam["DisplayComponents"] = DisplayComponents;
                dicParam["UnitOfMeasure"] = UnitOfMeasure;
                dicParam["Memo"] = Memo;
                dicParam["PackageWeight"] = PackageWeight;
                dicParam["PackageWeightUOM"] = PackageWeightUOM;
                dicParam["PackageLength"] = PackageLength;
                dicParam["PackageWidth"] = PackageWidth;
                dicParam["PackageHeight"] = PackageHeight;
                dicParam["PackageDimensionUOM"] = PackageDimensionUOM;
                dicParam["HSCode"] = HSCode;
                dicParam["GoodDescription"] = GoodDescription;
                dicParam["CountryOfOrigin"] = CountryOfOrigin;

                dicParam["CreatedBy"] = CreatedBy;
                ItemID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Item"), objConn, objTran).ToString();


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
                            objItemKit.ParentKey = ItemID;
                            objItemKit.ParentObject = "Item";
                            objItemKit.Create(objConn, objTran);
                        }
                    }
                }

                if (ItemComponents != null)
                {
                    foreach (ItemComponent objItemComponent in ItemComponents)
                    {
                        if (objItemComponent.IsNew)
                        {
                            objItemComponent.CompanyID = CompanyID;
                            objItemComponent.ItemID = ItemID;
                            objItemComponent.CreatedBy = CreatedBy;
                            objItemComponent.ParentKey = ItemID;
                            objItemComponent.ParentObject = "Item";
                            objItemComponent.Create(objConn, objTran);
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
                            objItemBarcode.ParentKey = ItemID;
                            objItemBarcode.ParentObject = "Item";
                            objItemBarcode.Create(objConn, objTran);
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

            LogAuditData(enumActionType.Create);
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
              
                dicParam["ItemParentID"] = ItemParentID;
                dicParam["ItemType"] = ItemType;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["ItemName"] = ItemName;
                dicParam["SalesDescription"] = SalesDescription;
                dicParam["PurchaseDescription"] = PurchaseDescription;
                dicParam["Barcode"] = Barcode;
                dicParam["IsBarcoded"] = IsBarcoded;
                dicParam["IsShipReceiveIndividually"] = IsShipReceiveIndividually;
                dicParam["FulfillByKit"] = FulfillByKit;
                dicParam["ReceiveByKit"] = ReceiveByKit;
                dicParam["DisplayComponents"] = DisplayComponents;
                dicParam["UnitOfMeasure"] = UnitOfMeasure;
                dicParam["Memo"] = Memo;
                dicParam["PackageWeight"] = PackageWeight;
                dicParam["PackageWeightUOM"] = PackageWeightUOM;
                dicParam["PackageLength"] = PackageLength;
                dicParam["PackageWidth"] = PackageWidth;
                dicParam["PackageHeight"] = PackageHeight;
                dicParam["PackageDimensionUOM"] = PackageDimensionUOM;
                dicParam["HSCode"] = HSCode;
                dicParam["GoodDescription"] = GoodDescription;
                dicParam["CountryOfOrigin"] = CountryOfOrigin;

                dicParam["UpdatedBy"] = UpdatedBy;
                dicWParam["ItemID"] = ItemID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Item"), objConn, objTran);

                Item currentItem = new Item(CompanyID, ItemID);

                foreach (ItemComponent _currentItemComponent in currentItem.ItemComponents)
                {
                    if (!ItemComponents.Exists(x => x.ItemComponentID == _currentItemComponent.ItemComponentID))
                    {
                        _currentItemComponent.UpdatedBy = UpdatedBy;
                        _currentItemComponent.ParentKey = ItemID;
                        _currentItemComponent.ParentObject = "Item";
                        _currentItemComponent.Delete(objConn, objTran);
                    }
                }

                if (ItemComponents != null)
                {
                    foreach (ItemComponent objItemComponent in ItemComponents)
                    {
                        if (objItemComponent.IsNew)
                        {
                            objItemComponent.CompanyID = CompanyID;
                            objItemComponent.ItemID = ItemID;
                            objItemComponent.CreatedBy = CreatedBy;
                            objItemComponent.ParentKey = ItemID;
                            objItemComponent.ParentObject = "Item";
                            objItemComponent.Create(objConn, objTran);
                        }
                        else
                        {
                            var matchingComponent = currentItem.ItemComponents.FirstOrDefault(x => x.ItemComponentID == objItemComponent.ItemComponentID);
                            if (matchingComponent != null)
                            {
                                matchingComponent.ChildItemID = objItemComponent.ChildItemID;
                                matchingComponent.Quantity = objItemComponent.Quantity;
                                matchingComponent.UpdatedBy = UpdatedBy;
                                matchingComponent.ParentKey = ItemID;
                                matchingComponent.ParentObject = "Item";                       
                                matchingComponent.Update(objConn, objTran);
                            }
                        }
                    }
                }

                //Delete no longer existing item attributevalueline for the item
                foreach (ItemKit _currentItemKit in currentItem.ItemKits)
                {
                    if (!ItemKits.Exists(x => x.ItemKitID == _currentItemKit.ItemKitID))
                    {
                        _currentItemKit.UpdatedBy = UpdatedBy;
                        _currentItemKit.ParentKey = ItemID;
                        _currentItemKit.ParentObject = "Item";
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
                            objItemKit.ParentKey = ItemID;
                            objItemKit.ParentObject = "Item";
                            objItemKit.Create(objConn, objTran);
                        }
                        else
                        {
                            var matchingItemKit = currentItem.ItemKits.FirstOrDefault(x => x.ItemKitID == objItemKit.ItemKitID);
                            if (matchingItemKit != null)
                            {
                                matchingItemKit.ChildItemID = objItemKit.ChildItemID;
                                matchingItemKit.Quantity = objItemKit.Quantity;
                                matchingItemKit.UpdatedBy = UpdatedBy;
                                matchingItemKit.ParentKey = ItemID;
                                matchingItemKit.ParentObject = "Item";
                                matchingItemKit.Update(objConn, objTran);
                            }                            
                        }
                    }
                }

                foreach (ItemBarcode _currentItemBarcode in currentItem.ItemBarcodes)
                {
                    if (!ItemBarcodes.Exists(x => x.ItemBarcodeID == _currentItemBarcode.ItemBarcodeID))
                    {
                        _currentItemBarcode.UpdatedBy = UpdatedBy;
                        _currentItemBarcode.ParentKey = ItemID;
                        _currentItemBarcode.ParentObject = "Item";
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
                            objItemBarcode.ParentKey = ItemID;
                            objItemBarcode.ParentObject = "Item";
                            objItemBarcode.Create(objConn, objTran);
                        }
                        else
                        {
                            var matchingItemBarcode = currentItem.ItemBarcodes.FirstOrDefault(x => x.ItemBarcodeID == objItemBarcode.ItemBarcodeID);
                            if (matchingItemBarcode != null)
                            {
                                matchingItemBarcode.Barcode = objItemBarcode.Barcode;
                                matchingItemBarcode.Type = objItemBarcode.Type;
                                matchingItemBarcode.UpdatedBy = UpdatedBy;
                                matchingItemBarcode.ParentKey = ItemID;
                                matchingItemBarcode.ParentObject = "Item";
                                matchingItemBarcode.Update(objConn, objTran);
                            }
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemID is missing");

                //foreach (ItemAttributeValueLine objItemAttributeValueLine in ItemAttributeValueLines)
                //{
                //    objItemAttributeValueLine.Delete(objConn, objTran);
                //}

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

            LogAuditData(enumActionType.Delete);
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM Item (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemNumber=" + Database.HandleQuote(ItemNumber);
          

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

                TotalRecord = objReturn.Count();
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
