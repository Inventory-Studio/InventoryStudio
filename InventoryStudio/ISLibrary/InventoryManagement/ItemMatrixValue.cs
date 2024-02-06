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
    public class ItemMatrixValue : BaseClass
    {
        public string ItemMatrixValueID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemMatrixValueID); } }
        public string CompanyID { get; set; }
        public string ItemMatrixID { get; set; }
        public string ItemAttributeID { get; set; }
        public string ItemAttributeValueID { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private ItemMatrix mItemMatrix = null;
        public ItemMatrix ItemMatrix
        {
            get
            {
                if (mItemMatrix == null && !string.IsNullOrEmpty(ItemMatrixID) && !string.IsNullOrEmpty(CompanyID))
                {
                    mItemMatrix = new ItemMatrix(CompanyID, ItemMatrixID);
                }
                return mItemMatrix;
            }
        }

        private ItemAttribute mItemAttribute = null;
        public ItemAttribute ItemAttribute
        {
            get
            {
                if (mItemAttribute == null && !string.IsNullOrEmpty(ItemAttributeID) && !string.IsNullOrEmpty(CompanyID))
                {
                    mItemAttribute = new ItemAttribute(CompanyID, ItemAttributeID);
                }
                return mItemAttribute;
            }
        }

        private ItemAttributeValue mItemAttributeValue = null;
        public ItemAttributeValue ItemAttributeValue
        {
            get
            {
                if (mItemAttributeValue == null && !string.IsNullOrEmpty(ItemAttributeValueID) && !string.IsNullOrEmpty(CompanyID))
                {
                    mItemAttributeValue = new ItemAttributeValue(CompanyID, ItemAttributeValueID);
                }
                return mItemAttributeValue;
            }
        }

        public ItemMatrixValue()
        {

        }

        public ItemMatrixValue(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemMatrixValue(string CompanyID, string ItemMatrixValueID)
        {
            this.CompanyID = CompanyID;
            this.ItemMatrixValueID = ItemMatrixValueID;
            Load();
        }

        public ItemMatrixValue(DataRow objRow)
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
                strSQL = "SELECT * FROM ItemMatrixValue (NOLOCK) WHERE ItemMatrixValueID=" + Database.HandleQuote(ItemMatrixValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemMatrixValueID=" + ItemMatrixValueID + " is not found");
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

                if (objColumns.Contains("ItemMatrixValueID")) ItemMatrixValueID = Convert.ToString(objRow["ItemMatrixValueID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemMatrixID")) ItemMatrixID = Convert.ToString(objRow["ItemMatrixID"]);
                if (objColumns.Contains("ItemAttributeID")) ItemAttributeID = Convert.ToString(objRow["ItemAttributeID"]);
                if (objColumns.Contains("ItemAttributeValueID")) ItemAttributeValueID = Convert.ToString(objRow["ItemAttributeValueID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemMatrixValueID)) throw new Exception("Missing ItemMatrixValueID in the datarow");
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
                if (string.IsNullOrEmpty(ItemMatrixID)) throw new Exception("ItemMatrixID must be entered");
                if (string.IsNullOrEmpty(ItemAttributeID)) throw new Exception("ItemAttributeID must be entered");
                if (string.IsNullOrEmpty(ItemAttributeValueID)) throw new Exception("ItemAttributeValueID must be entered");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy name must be entered");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemMatrixValueID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemMatrixID"] = ItemMatrixID;
                dicParam["ItemAttributeID"] = ItemAttributeID;
                dicParam["ItemAttributeValueID"] = ItemAttributeValueID; //ItemAttributeValue.ItemAttributeValueID;
                dicParam["CreatedBy"] = CreatedBy;
                ItemMatrixValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemMatrixValue"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(ItemMatrixID)) throw new Exception("ItemMatrixID must be entered");
                if (string.IsNullOrEmpty(ItemAttributeID)) throw new Exception("ItemAttributeID must be entered");
                if (string.IsNullOrEmpty(ItemAttributeValueID)) throw new Exception("ItemAttributeValueID must be entered");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy name must be entered");
                if (IsNew) throw new Exception("Update cannot be performed, ItemMatrixValueID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemMatrixID"] = ItemMatrixID;
                dicParam["ItemAttributeID"] = ItemAttributeID;
                dicParam["ItemAttributeValueID"] = ItemAttributeValueID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicWParam["ItemMatrixValueID"] = ItemMatrixValueID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemMatrixValue"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemMatrixValueID is missing");

                dicDParam["ItemMatrixValueID"] = ItemMatrixValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemMatrixValue"), objConn, objTran);
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
                     "FROM ItemMatrixValue (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemAttributeID=" + Database.HandleQuote(ItemAttributeID) +
                     "AND p.ItemAttributeValueID=" + Database.HandleQuote(ItemAttributeValueID);

            if (!string.IsNullOrEmpty(ItemMatrixValueID)) strSQL += "AND p.ItemMatrixValueID<>" + Database.HandleQuote(ItemMatrixValueID);
            return Database.HasRows(strSQL);
        }

        public static ItemMatrixValue GetItemMatrixValue(string CompanyID, ItemMatrixValueFilter Filter)
        {
            List<ItemMatrixValue> objGetItemMatrixValues = null;
            ItemMatrixValue objReturn = null;

            try
            {
                objGetItemMatrixValues = GetItemMatrixValues(CompanyID, Filter);
                if (objGetItemMatrixValues != null && objGetItemMatrixValues.Count >= 1) objReturn = objGetItemMatrixValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objGetItemMatrixValues = null;
            }
            return objReturn;
        }

        public static List<ItemMatrixValue> GetItemMatrixValues(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemMatrixValues(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemMatrixValue> GetItemMatrixValues(string CompanyID, ItemMatrixValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemMatrixValues(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemMatrixValue> GetItemMatrixValues(string CompanyID, ItemMatrixValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemMatrixValues(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemMatrixValue> GetItemMatrixValues(string CompanyID, ItemMatrixValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemMatrixValue> objReturn = null;
            ItemMatrixValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<ItemMatrixValue>();

                    strSQL = "SELECT i.* " +
                             "FROM ItemMatrixValue i (NOLOCK) " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.ItemMatrixID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemMatrixID, "i.ItemMatrixID");
                        if (Filter.ItemAttributeValueID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemAttributeValueID, "i.ItemAttributeValueID");
                    }

                    if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeName" : Utility.CustomSorting.GetSortExpression(typeof(ItemMatrixValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new ItemMatrixValue(objData.Tables[0].Rows[i]);
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
    }
}