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
    public class ItemAttributeValueLine : BaseClass
    {
        public string ItemAttributeValueLineID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemAttributeValueLineID); } }
        public string CompanyID { get; set; }
        public string ItemID { get; set; }
        public string ItemAttributeValueID { get; set; }
        //public ItemAttributeValue ItemAttributeValue { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private ItemAttributeValue mItemAttributeValue = null;
        public ItemAttributeValue ItemAttributeValue
        {
            get
            {
                if (mItemAttributeValue == null && IsLoaded && !string.IsNullOrEmpty(ItemAttributeValueID) && !string.IsNullOrEmpty(CompanyID))
                {
                    mItemAttributeValue = new ItemAttributeValue(CompanyID, ItemAttributeValueID);
                }
                return mItemAttributeValue;
            }
        }

        public ItemAttributeValueLine()
        {

        }

        public ItemAttributeValueLine(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemAttributeValueLine(string CompanyID, string ItemAttributeValueLineID)
        {
            this.CompanyID = CompanyID;
            this.ItemAttributeValueLineID = ItemAttributeValueLineID;
            Load();
        }

        public ItemAttributeValueLine(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * FROM ItemAttributeValueLine (NOLOCK) WHERE ItemAttributeValueLineID=" + Database.HandleQuote(ItemAttributeValueLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemAttributeValueLineID=" + ItemAttributeValueLineID + " is not found");
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

                if (objColumns.Contains("ItemAttributeValueLineID")) ItemAttributeValueLineID = Convert.ToString(objRow["ItemAttributeValueLineID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ItemAttributeValueID")) ItemAttributeValueID = Convert.ToString(objRow["ItemAttributeValueID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemAttributeValueLineID)) throw new Exception("Missing ItemAttributeValueLineID in the datarow");
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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID name must be entered");
                //if (ItemAttributeValue == null) throw new Exception("ItemAttributeValue is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy name must be entered");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemAttributeValueLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ItemAttributeValueID"] = ItemAttributeValueID; //ItemAttributeValue.ItemAttributeValueID;
                dicParam["CreatedBy"] = CreatedBy;
                ItemAttributeValueLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemAttributeValueLine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID name must be entered");
                if (string.IsNullOrEmpty(ItemAttributeValueID)) throw new Exception("ItemAttributeValueID name must be entered");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy name must be entered");
                if (IsNew) throw new Exception("Update cannot be performed, ItemAttributeValueLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ItemAttributeValueID"] = ItemAttributeValueID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicWParam["ItemAttributeValueLineID"] = ItemAttributeValueLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemAttributeValueLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemAttributeValueLineID is missing");

                dicDParam["ItemAttributeValueLineID"] = ItemAttributeValueLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemAttributeValueLine"), objConn, objTran);
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
                     "FROM ItemAttributeValueLine (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemID=" + Database.HandleQuote(ItemID) +
                     "AND p.ItemAttributeValueID=" + Database.HandleQuote(ItemAttributeValueID);

            if (!string.IsNullOrEmpty(ItemAttributeValueLineID)) strSQL += "AND p.ItemAttributeValueLineID<>" + Database.HandleQuote(ItemAttributeValueLineID);
            return Database.HasRows(strSQL);
        }

        public static ItemAttributeValueLine GetItemAttributeValueLine(string CompanyID, ItemAttributeValueLineFilter Filter)
        {
            List<ItemAttributeValueLine> objItemAttributeValueLines = null;
            ItemAttributeValueLine objReturn = null;

            try
            {
                objItemAttributeValueLines = GetItemAttributeValueLines(CompanyID, Filter);
                if (objItemAttributeValueLines != null && objItemAttributeValueLines.Count >= 1) objReturn = objItemAttributeValueLines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemAttributeValueLines = null;
            }
            return objReturn;
        }

        public static List<ItemAttributeValueLine> GetItemAttributeValueLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemAttributeValueLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemAttributeValueLine> GetItemAttributeValueLines(string CompanyID, ItemAttributeValueLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemAttributeValueLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemAttributeValueLine> GetItemAttributeValueLines(string CompanyID, ItemAttributeValueLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemAttributeValueLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemAttributeValueLine> GetItemAttributeValueLines(string CompanyID, ItemAttributeValueLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemAttributeValueLine> objReturn = null;
            ItemAttributeValueLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<ItemAttributeValueLine>();

                    strSQL = "SELECT i.* " +
                             "FROM ItemAttributeValueLine i (NOLOCK) " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "i.ItemID");
                        if (Filter.ItemAttributeValueID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemAttributeValueID, "i.ItemAttributeValueID");
                    }

                    if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeName" : Utility.CustomSorting.GetSortExpression(typeof(ItemAttributeValueLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new ItemAttributeValueLine(objData.Tables[0].Rows[i]);
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
    }
}
