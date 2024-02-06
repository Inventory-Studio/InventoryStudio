using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;

namespace ISLibrary
{
    public class ItemParent : BaseClass
    {
        public string ItemParentID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemParentID); } }
        public string CompanyID { get; set; }
        public string ItemNumber { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public bool IsVariation
        {
            get
            {
                return ItemAttributes != null && ItemAttributes.Count > 0;
            }
        }

        private List<Item> mItems = null;
        public List<Item> Items
        {
            get
            {
                ItemFilter objFilter = null;

                try
                {
                    if (mItems == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemParentID))
                    {
                        objFilter = new ItemFilter();
                        objFilter.ItemParentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemParentID.SearchString = ItemParentID;
                        mItems = Item.GetItems(CompanyID, objFilter);
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
                return mItems;
            }
            set
            {
                mItems = value;
            }
        }

        private List<ItemAttribute> mItemAttributes = null;
        public List<ItemAttribute> ItemAttributes
        {
            get
            {
                ItemAttributeFilter objFilter = null;

                try
                {
                    if (mItemAttributes == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemParentID))
                    {
                        objFilter = new ItemAttributeFilter();
                        objFilter.ItemParentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemParentID.SearchString = ItemParentID;
                        mItemAttributes = ItemAttribute.GetItemAttributes(CompanyID, objFilter);
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
                return mItemAttributes;
            }
            set
            {
                mItemAttributes = value;
            }
        }

        private List<ItemMatrix> mItemMatrices = null;
        public List<ItemMatrix> ItemMatrices
        {
            get
            {
                ItemMatrixFilter objFilter = null;

                try
                {
                    if (mItemMatrices == null)
                    {
                        if (!string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemParentID))
                        {
                            objFilter = new ItemMatrixFilter();
                            objFilter.ItemParentID = new Database.Filter.StringSearch.SearchFilter();
                            objFilter.ItemParentID.SearchString = ItemParentID;
                            mItemMatrices = ItemMatrix.GetItemMatrices(CompanyID, objFilter);
                        }
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
                return mItemMatrices;
            }
            set
            {
                mItemMatrices = value;
            }
        }

        public ItemParent()
        {
        }
        public ItemParent(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemParent(string CompanyID, string ItemParentID)
        {
            this.CompanyID = CompanyID;
            this.ItemParentID = ItemParentID;
            Load();
        }

        public ItemParent(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * FROM ItemParent (NOLOCK) WHERE ItemParentID=" + Database.HandleQuote(ItemParentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemParentID=" + ItemParentID + " is not found");
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
            base.Load();
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("ItemParentID")) ItemParentID = Convert.ToString(objRow["ItemParentID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
               
                if (objColumns.Contains("ItemNumber")) ItemNumber = Convert.ToString(objRow["ItemNumber"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemParentID)) throw new Exception("Missing ItemParentID in the datarow");
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

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID must be entered");
                if (string.IsNullOrEmpty(ItemNumber)) throw new Exception("ItemNumber must be entered");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy name must be entered");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemParentID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["CreatedBy"] = CreatedBy;
                ItemParentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemParent"), objConn, objTran).ToString();

                if (ItemAttributes != null)
                {
                    foreach (ItemAttribute objItemAttribute in ItemAttributes)
                    {
                        if(objItemAttribute.IsNew)
                        {
                            objItemAttribute.ItemParentID = ItemParentID;
                            objItemAttribute.CompanyID = CompanyID;
                            objItemAttribute.CreatedBy = CreatedBy;
                            objItemAttribute.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemAttribute.ItemParentID = ItemParentID;
                            objItemAttribute.UpdatedBy = CreatedBy;
                            objItemAttribute.Update(objConn, objTran);
                        }
                    }
                }

                if (ItemMatrices != null)
                {
                    foreach (ItemMatrix objItemMatrix in ItemMatrices)
                    {
                        // Split the AttributeValue by '-' to get individual attribute values
                        string[] attributeValues = objItemMatrix.AttributeValue.Split('-');

                        
                        List<ItemMatrixValue> objItemMatrixValues = new List<ItemMatrixValue>();

                        foreach (string attributeValue in attributeValues)
                        {
                            // Assuming ItemAttributes is a collection of objItemAttribute
                            // and each objItemAttribute has a collection of ItemAttributeValue
                            var itemAttribute = ItemAttributes
                                .FirstOrDefault(ia => ia.ItemAttributeValues.Any(ava => ava.AttributeValueName == attributeValue));

                            if (itemAttribute != null)
                            {
                                // Assuming ItemAttributeValue is a collection and we need the first or default
                                var itemAttributeValue = itemAttribute.ItemAttributeValues
                                    .FirstOrDefault(ava => ava.AttributeValueName == attributeValue);

                                if (itemAttributeValue != null)
                                {
                                    ItemMatrixValue objItemMatrixValue = new ItemMatrixValue();
                                    objItemMatrixValue.ItemAttributeID = itemAttribute.ItemAttributeID;
                                    objItemMatrixValue.ItemAttributeValueID = itemAttributeValue.ItemAttributeValueID;
                                    objItemMatrixValues.Add(objItemMatrixValue);
                                }
                            }
                        }

                        if (objItemMatrixValues.Count > 0)
                        {
                            objItemMatrix.ItemMatrixValues = objItemMatrixValues;
                        }

                        if (objItemMatrix.IsNew)
                        {
                            objItemMatrix.CompanyID = CompanyID;
                            objItemMatrix.ItemParentID = ItemParentID;
                            objItemMatrix.CreatedBy = CreatedBy;
                            objItemMatrix.CreatedBy = CreatedBy;
                            objItemMatrix.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemMatrix.UpdatedBy = CreatedBy;
                            objItemMatrix.Update(objConn, objTran);
                        }
                    }
                }



                if (Items != null)
                {
                    foreach (Item objItem in Items)
                    {
                        if(objItem.IsNew)
                        {
                            if (!IsVariation) objItem.ItemNumber = ItemNumber;
                            objItem.ItemParentID = ItemParentID;
                            objItem.CompanyID = CompanyID;
                            objItem.CreatedBy = CreatedBy;
                            objItem.Create(objConn, objTran);
                        }
                        else
                        {
                            objItem.ItemParentID = ItemParentID;
                            objItem.UpdatedBy = CreatedBy;
                            objItem.Update(objConn, objTran);
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
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID must be entered");
                if (string.IsNullOrEmpty(ItemNumber)) throw new Exception("ItemNumber must be entered");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy must be entered");
                if (IsNew) throw new Exception("Update cannot be performed, ItemParentID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicWParam["ItemParentID"] = ItemParentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemParent"), objConn, objTran);

                if (ItemAttributes != null)
                {
                    foreach (ItemAttribute objItemAttribute in ItemAttributes)
                    {
                        if (objItemAttribute.IsNew)
                        {
                            objItemAttribute.ItemParentID = ItemParentID;
                            objItemAttribute.CreatedBy = UpdatedBy;
                            objItemAttribute.Create(objConn, objTran);
                        }
                        else
                        {
                            objItemAttribute.UpdatedBy = UpdatedBy;
                            objItemAttribute.Update(objConn, objTran);
                        }
                    }
                }

                if (Items != null)
                {
                    foreach (Item objItem in Items)
                    {
                        if (!IsVariation) objItem.ItemNumber = ItemNumber;
                        if (objItem.IsNew)
                        {
                            objItem.ItemParentID = ItemParentID;
                            objItem.CompanyID = CompanyID;
                            objItem.CreatedBy = UpdatedBy;
                            objItem.Create(objConn, objTran);
                        }
                        else
                        {
                            objItem.UpdatedBy = UpdatedBy;
                            objItem.Update(objConn, objTran);
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
            base.Update();
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
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ItemParentID is missing");

                foreach (Item objItem in Items)
                {
                    objItem.Delete(objConn, objTran);
                }

                foreach (ItemAttribute objItemAttribute in ItemAttributes)
                {
                    objItemAttribute.Delete(objConn, objTran);
                }

                dicDParam["ItemParentID"] = ItemParentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemParent"), objConn, objTran);
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
                     "FROM ItemParent (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemNumber=" + Database.HandleQuote(ItemNumber);
            
            if (!string.IsNullOrEmpty(ItemParentID)) strSQL += "AND p.ItemParentID<>" + Database.HandleQuote(ItemParentID);
            return Database.HasRows(strSQL);
        }

        public static ItemParent GetItemParent(string CompanyID, ItemParentFilter Filter)
        {
            List<ItemParent> objItemParents = null;
            ItemParent objReturn = null;

            try
            {
                objItemParents = GetItemParents(CompanyID, Filter);
                if (objItemParents != null && objItemParents.Count >= 1) objReturn = objItemParents[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemParents = null;
            }
            return objReturn;
        }

        public static List<ItemParent> GetItemParents(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemParents(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemParent> GetItemParents(string CompanyID, ItemParentFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemParents(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemParent> GetItemParents(string CompanyID, ItemParentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemParents(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemParent> GetItemParents(string CompanyID, ItemParentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemParent> objReturn = null;
            ItemParent objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<ItemParent>();

                    strSQL = "SELECT i.* " +
                             "FROM ItemParent i (NOLOCK) " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.ClientID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ClientID, "i.ClientID");
                        if (Filter.ItemNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemNumber, "i.ItemNumber");
                    }

                    if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeName" : Utility.CustomSorting.GetSortExpression(typeof(ItemParent), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new ItemParent(objData.Tables[0].Rows[i]);
                            objReturn.Add(objNew);
                        }
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

        public static Item CreateItem(Item item,List<ItemAttribute> itemAttributes,List<ItemMatrix> itemMatrices)
        {
            ItemParent itemParent = new ItemParent();
            itemParent.CompanyID = item.CompanyID;
            itemParent.ItemNumber = item.ItemNumber;
            itemParent.CreatedBy = item.CreatedBy;

            itemParent.Items = new List<Item> { item };
            itemParent.ItemAttributes = itemAttributes;
            itemParent.ItemMatrices = itemMatrices;
            itemParent.Create();
            return item;
        }

    }
}
