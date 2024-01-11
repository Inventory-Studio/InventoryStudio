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
    public class ItemComponent : BaseClass
    {
        public string ItemComponentID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemComponentID); } }
        public string CompanyID { get; set; }
        public string ItemID { get; set; }
        public string ChildItemID { get; set; }
        public int Quantity { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }


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
        public ItemComponent()
        {
        }

        public ItemComponent(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemComponent(string CompanyID, string ItemComponentID)
        {
            this.CompanyID = CompanyID;
            this.ItemComponentID = ItemComponentID;
            Load();
        }

        public ItemComponent(DataRow objRow)
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
                         "FROM ItemComponent (NOLOCK) " +
                         "WHERE ItemComponentID=" + Database.HandleQuote(ItemComponentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemComponentID=" + ItemComponentID + " is not found");
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

                if (objColumns.Contains("ItemComponentID")) ItemComponentID = Convert.ToString(objRow["ItemComponentID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ChildItemID")) ChildItemID = Convert.ToString(objRow["ChildItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemComponentID)) throw new Exception("Missing ItemComponentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, ItemComponentID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ChildItemID"] = ChildItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["CreatedBy"] = CreatedBy;
                ItemComponentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemComponent"), objConn, objTran).ToString();
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
                dicWParam["ItemComponentID"] = ItemComponentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemComponent"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemComponentID is missing");

                dicDParam["ItemComponentID"] = ItemComponentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemComponent"), objConn, objTran);
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
                     "FROM ItemComponent (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemID=" + Database.HandleQuote(ItemID) +
                     "AND p.ChildItemID=" + Database.HandleQuote(ChildItemID);

            if (!string.IsNullOrEmpty(ItemComponentID)) strSQL += "AND p.ItemComponentID<>" + Database.HandleQuote(ItemComponentID);
            return Database.HasRows(strSQL);
        }

        public static ItemComponent GetItemComponent(string CompanyID, ItemComponentFilter Filter)
        {
            List<ItemComponent> objItemComponents = null;
            ItemComponent objReturn = null;

            try
            {
                objItemComponents = GetItemComponents(CompanyID, Filter);
                if (objItemComponents != null && objItemComponents.Count >= 1) objReturn = objItemComponents[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemComponents = null;
            }
            return objReturn;
        }

        public static List<ItemComponent> GetItemComponents(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemComponents(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemComponent> GetItemComponents(string CompanyID, ItemComponentFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemComponents(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemComponent> GetItemComponents(string CompanyID, ItemComponentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemComponents(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemComponent> GetItemComponents(string CompanyID, ItemComponentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemComponent> objReturn = null;
            ItemComponent objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemComponent>();

                strSQL = "SELECT * " +
                         "FROM ItemComponent (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemComponentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemComponentID, "ItemComponentID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.ChildItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ChildItemID, "ChildItemID");
                    if (Filter.Quantity != null) strSQL += Database.Filter.NumericSearch.GetSQLQuery(Filter.Quantity, "Quantity");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemComponentID" : Utility.CustomSorting.GetSortExpression(typeof(ItemComponent), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemComponent(objData.Tables[0].Rows[i]);
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
