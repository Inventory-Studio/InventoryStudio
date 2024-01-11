using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;

namespace ISLibrary
{
    public class ItemKit : BaseClass
    {
        public string ItemKitID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemKitID); } }
        public string CompanyID { get; set; }
        public string ItemID { get; set; }
        public string ChildItemID { get; set; }
        public decimal Quantity { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        //private Item mItem= null;
        //public Item Item
        //{
        //    get
        //    {
        //        if (mItem == null && IsLoaded && !string.IsNullOrEmpty(ItemID) && !string.IsNullOrEmpty(CompanyID))
        //        {
        //            mItem = new Item(CompanyID, ItemID);
        //        }
        //        return mItem;
        //    }
        //}

        private Item mChildItem = null;
        public Item ChildItem
        {
            get
            {
                if (mChildItem == null && IsLoaded && !string.IsNullOrEmpty(ChildItemID) && !string.IsNullOrEmpty(CompanyID))
                {
                    mChildItem = new Item(CompanyID, ChildItemID);
                }
                return mChildItem;
            }
        }
        public ItemKit()
        {
        }

        public ItemKit(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemKit(string CompanyID, string ItemKitID)
        {
            this.CompanyID = CompanyID;
            this.ItemKitID = ItemKitID;
            Load();
        }

        public ItemKit(DataRow objRow)
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
                strSQL = "SELECT * " +
                         "FROM ItemKit (NOLOCK) " +
                         "WHERE ItemKitID=" + Database.HandleQuote(ItemKitID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemKitID=" + ItemKitID + " is not found");
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

                if (objColumns.Contains("ItemKitID")) ItemKitID = Convert.ToString(objRow["ItemKitID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ChildItemID")) ChildItemID = Convert.ToString(objRow["ChildItemID"]);
                if (objColumns.Contains("Quantity") && objRow["Quantity"] != DBNull.Value) Quantity = Convert.ToDecimal(objRow["Quantity"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemKitID)) throw new Exception("Missing ItemKitID in the datarow");
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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(ChildItemID)) throw new Exception("ChildItemID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemKitID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ChildItemID"] = ChildItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["CreatedBy"] = CreatedBy;
                ItemKitID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemKit"), objConn, objTran).ToString();
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
            base.Update();

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(ChildItemID)) throw new Exception("ChildItemID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ChildItemID"] = ChildItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemKitID"] = ItemKitID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemKit"), objConn, objTran);
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
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ItemKitID is missing");

                dicDParam["ItemKitID"] = ItemKitID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemKit"), objConn, objTran);
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
                     "FROM ItemKit (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemID=" + Database.HandleQuote(ItemID) +
                     "AND p.ChildItemID=" + Database.HandleQuote(ChildItemID);

            if (!string.IsNullOrEmpty(ItemKitID)) strSQL += "AND p.ItemKitID<>" + Database.HandleQuote(ItemKitID);
            return Database.HasRows(strSQL);
        }

        public static ItemKit GetItemKit(string CompanyID, ItemKitFilter Filter)
        {
            List<ItemKit> objItemKits = null;
            ItemKit objReturn = null;

            try
            {
                objItemKits = GetItemKits(CompanyID, Filter);
                if (objItemKits != null && objItemKits.Count >= 1) objReturn = objItemKits[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemKits = null;
            }
            return objReturn;
        }

        public static List<ItemKit> GetItemKits(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemKits(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemKit> GetItemKits(string CompanyID, ItemKitFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemKits(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemKit> GetItemKits(string CompanyID, ItemKitFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemKits(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemKit> GetItemKits(string CompanyID, ItemKitFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemKit> objReturn = null;
            ItemKit objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemKit>();

                strSQL = "SELECT * " +
                         "FROM ItemKit (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemKitID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemKitID, "ItemKitID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.ChildItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ChildItemID, "ChildItemID");
                    if (Filter.Quantity != null) strSQL += Database.Filter.NumericSearch.GetSQLQuery(Filter.Quantity, "Quantity");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemKitID" : Utility.CustomSorting.GetSortExpression(typeof(ItemKit), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemKit(objData.Tables[0].Rows[i]);
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
            }
            return objReturn;
        }
    }
}
