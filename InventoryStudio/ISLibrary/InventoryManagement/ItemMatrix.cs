using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Net.NetworkInformation;
using CLRFramework;
using Microsoft.Data.SqlClient;

namespace ISLibrary
{
    public class ItemMatrix : BaseClass
    {
        public string ItemMatrixID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemMatrixID); } }
        public string CompanyID { get; set; }
        public string ItemParentID { get; set; }
        public string ItemID { get; set; }
        public string AttributeValue { get; set; }
        public string UpdatedBy { get; set; }
        //used for frontedPage to create child Item, if true and itemID is null, then create new item
        public bool IsChecked { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private List<ItemMatrixValue> mItemMatrixValues = null;
        public List<ItemMatrixValue> ItemMatrixValues
        {
            get
            {
                ItemMatrixValueFilter objFilter = null;

                try
                {
                    if (mItemMatrixValues == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemMatrixID))
                    {
                        objFilter = new ItemMatrixValueFilter();
                        objFilter.ItemMatrixID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemMatrixID.SearchString = ItemMatrixID;
                        mItemMatrixValues = ItemMatrixValue.GetItemMatrixValues(CompanyID, objFilter);
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
                return mItemMatrixValues;
            }
            set
            {
                mItemMatrixValues = value;
            }
        }

        private Item mItem = null;
        public Item Item
        {
            get
            {

                try
                {
                    if (mItem == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemID))
                    {
                        mItem = new Item(CompanyID,ItemID);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return mItem;
            }
            set
            {
                mItem = value;
            }
        }

        public string AttributeValueCombination => GetAttributeValueCombination();
        public ItemMatrix()
        {
        }

        public ItemMatrix(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemMatrix(string CompanyID, string ItemMatrixID)
        {
            this.CompanyID = CompanyID;
            this.ItemMatrixID = ItemMatrixID;
            Load();
        }

        public ItemMatrix(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * FROM ItemMatrix (NOLOCK) WHERE ItemMatrixID=" + Database.HandleQuote(ItemMatrixID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemMatrixID=" + ItemMatrixID + " is not found");
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

                if (objColumns.Contains("ItemMatrixID")) ItemMatrixID = Convert.ToString(objRow["ItemMatrixID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemParentID")) ItemParentID = Convert.ToString(objRow["ItemParentID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemMatrixID)) throw new Exception("Missing ItemMatrixID in the datarow");
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

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(ItemParentID)) throw new Exception("ItemParentID must be entered");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy name must be entered");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemMatrixID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemParentID"] = ItemParentID;
                dicParam["ItemID"] = ItemID;
                dicParam["CreatedBy"] = CreatedBy;
                ItemMatrixID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemMatrix"), objConn, objTran).ToString();

                if (ItemMatrixValues != null)
                {
                    foreach (ItemMatrixValue objItemMatrixValue in ItemMatrixValues)
                    {
                        objItemMatrixValue.ItemMatrixID = ItemMatrixID;
                        objItemMatrixValue.CompanyID = CompanyID;
                        objItemMatrixValue.CreatedBy = CreatedBy;
                        objItemMatrixValue.ParentKey = ItemParentID;
                        objItemMatrixValue.ParentObject = "ItemParent";
                        objItemMatrixValue.Create(objConn, objTran);
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

            try
            {
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy name must be entered");
                if (IsNew) throw new Exception("Update cannot be performed, ItemMatrixID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

             
                dicParam["ItemID"] = ItemID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemMatrixID"] = ItemMatrixID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemMatrix"), objConn, objTran);

                //foreach (ItemMatrixValue objItemMatrixValue in ItemMatrixValues)
                //{
                //    if (objItemMatrixValue.IsNew)
                //    {
                //        objItemMatrixValue.CreatedBy = UpdatedBy;
                //        objItemMatrixValue.Create(objConn, objTran);
                //    }
                //    else
                //    {
                //        objItemMatrixValue.UpdatedBy = UpdatedBy;
                //        objItemMatrixValue.Update(objConn, objTran);
                //    }
                //}

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
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ItemMatrixID is missing");

                foreach (ItemMatrixValue objValue in ItemMatrixValues)
                {
                    objValue.Delete(objConn, objTran);
                }
                dicDParam["ItemMatrixID"] = ItemMatrixID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemMatrix"), objConn, objTran);
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
            if (string.IsNullOrEmpty(ItemID))
            {
                return false;
            }
            else
            {
                string strSQL = string.Empty;

                strSQL = "SELECT TOP 1 p.* " +
                         "FROM ItemMatrix (NOLOCK) p " +
                         "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND p.ItemID=" + Database.HandleQuote(ItemID) +
                         "AND p.ItemParentID=" + Database.HandleQuote(ItemParentID);

                if (!string.IsNullOrEmpty(ItemMatrixID)) strSQL += "AND p.ItemMatrixID<>" + Database.HandleQuote(ItemMatrixID);
                return Database.HasRows(strSQL);
            }
        }

        public static ItemMatrix GetItemMatrix(string CompanyID, ItemMatrixFilter Filter)
        {
            List<ItemMatrix> objItemMatrices = null;
            ItemMatrix objReturn = null;

            try
            {
                objItemMatrices = GetItemMatrices(CompanyID, Filter);
                if (objItemMatrices != null && objItemMatrices.Count >= 1) objReturn = objItemMatrices[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemMatrices = null;
            }
            return objReturn;
        }

        public static List<ItemMatrix> GetItemMatrices(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemMatrices(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemMatrix> GetItemMatrices(string CompanyID, ItemMatrixFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemMatrices(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemMatrix> GetItemMatrices(string CompanyID, ItemMatrixFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemMatrices(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemMatrix> GetItemMatrices(string CompanyID, ItemMatrixFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemMatrix> objReturn = null;
            ItemMatrix objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<ItemMatrix>();

                    strSQL = "SELECT i.* " +
                             "FROM ItemMatrix i (NOLOCK) " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.ItemParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemParentID, "i.ItemParentID");
                        if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "i.ItemID");
                    }

                    if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeName" : Utility.CustomSorting.GetSortExpression(typeof(ItemMatrix), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    else strSQL += " Order by ItemMatrixID";

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new ItemMatrix(objData.Tables[0].Rows[i]);
                            objNew.IsLoaded = true;
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

        public string GetAttributeValueCombination()
        {
            if (ItemMatrixValues == null || !ItemMatrixValues.Any())
            {
                return string.Empty;
            }
       
            var attributeValueCombination = string.Join("-",
                ItemMatrixValues
                    .Select(v => v.ItemAttributeValue?.AttributeValueName ?? string.Empty)
                    .Where(name => !string.IsNullOrEmpty(name)));

            return attributeValueCombination;
        }
    }
}