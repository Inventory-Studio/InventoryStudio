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
    public class ItemAttribute : BaseClass
    {
        public string ItemAttributeID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemAttributeID); } }
        public string CompanyID { get; set; }
        public string ClientID { get; set; }
        public string ItemParentID { get; set; }
        public string AttributeName { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private List<ItemAttributeValue> mItemAttributeValues = null;
        public List<ItemAttributeValue> ItemAttributeValues
        {
            get
            {
                ItemAttributeValueFilter objFilter = null;

                try
                {
                    if (mItemAttributeValues == null && IsLoaded && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemAttributeID))
                    {
                        objFilter = new ItemAttributeValueFilter();
                        objFilter.ItemAttributeID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemAttributeID.SearchString = ItemAttributeID;
                        mItemAttributeValues = ItemAttributeValue.GetItemAttributeValues(CompanyID, objFilter);
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
                return mItemAttributeValues;
            }
            set
            {
                mItemAttributeValues = value;
            }
        }

        public ItemAttribute()
        {
        }

        public ItemAttribute(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemAttribute(string CompanyID, string ItemAttributeID)
        {
            this.CompanyID = CompanyID;
            this.ItemAttributeID = ItemAttributeID;
            Load();
        }

        public ItemAttribute(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * FROM ItemAttribute (NOLOCK) WHERE ItemAttributeID=" + Database.HandleQuote(ItemAttributeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemAttributeID=" + ItemAttributeID + " is not found");
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
            base.Load();

            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("ItemAttributeID")) ItemAttributeID = Convert.ToString(objRow["ItemAttributeID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ClientID")) ClientID = Convert.ToString(objRow["ClientID"]);
                if (objColumns.Contains("ItemParentID")) ItemParentID = Convert.ToString(objRow["ItemParentID"]);
                if (objColumns.Contains("AttributeName")) AttributeName = Convert.ToString(objRow["AttributeName"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemAttributeID)) throw new Exception("Missing ItemAttributeID in the datarow");
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
                if (string.IsNullOrEmpty(AttributeName.Trim())) throw new Exception("Attribute name must be entered");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy name must be entered");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemAttributeID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ClientID"] = ClientID;
                dicParam["ItemParentID"] = ItemParentID;
                dicParam["AttributeName"] = AttributeName;
                dicParam["CreatedBy"] = CreatedBy;
                ItemAttributeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemAttribute"), objConn, objTran).ToString();

                foreach (ItemAttributeValue objItemAttributeValue in ItemAttributeValues)
                {
                    objItemAttributeValue.ItemAttributeID = ItemAttributeID;
                    objItemAttributeValue.CompanyID = CompanyID;
                    objItemAttributeValue.CreatedBy = CreatedBy;
                    objItemAttributeValue.Create(objConn, objTran);
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
                //if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID name must be entered");
                if (string.IsNullOrEmpty(AttributeName.Trim())) throw new Exception("Attribute name must be entered");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy name must be entered");
                if (IsNew) throw new Exception("Update cannot be performed, ItemAttributeID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                //dicParam["CompanyID"] = CompanyID;
                //dicParam["ClientID"] = ClientID;
                //dicParam["ItemParentID"] = ItemParentID;
                dicParam["AttributeName"] = AttributeName;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemAttributeID"] = ItemAttributeID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemAttribute"), objConn, objTran);

                ItemAttribute currentItemAttribute = new ItemAttribute(CompanyID, ItemAttributeID);

                foreach (ItemAttributeValue _currentItemAttributeValue in currentItemAttribute.ItemAttributeValues)
                {
                    if (!ItemAttributeValues.Exists(x => x.ItemAttributeValueID == _currentItemAttributeValue.ItemAttributeValueID))
                    {
                        _currentItemAttributeValue.Delete(objConn, objTran);
                    }
                }

                foreach (ItemAttributeValue objItemAttributeValue in ItemAttributeValues)
                {
                    if (objItemAttributeValue.IsNew)
                    {
                        objItemAttributeValue.ItemAttributeID = ItemAttributeID;
                        objItemAttributeValue.CompanyID = CompanyID;
                        objItemAttributeValue.CreatedBy = UpdatedBy;
                        objItemAttributeValue.Create(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemAttributeID is missing");

                foreach (ItemAttributeValue objValue in ItemAttributeValues)
                {
                    objValue.Delete(objConn, objTran);
                }
                dicDParam["ItemAttributeID"] = ItemAttributeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemAttribute"), objConn, objTran);
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
            if (string.IsNullOrEmpty(ItemParentID))
            {
                return false;
            }
            else
            {
                string strSQL = string.Empty;

                strSQL = "SELECT TOP 1 p.* " +
                         "FROM ItemAttribute (NOLOCK) p " +
                         "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND p.AttributeName=" + Database.HandleQuote(AttributeName);
                if (!string.IsNullOrEmpty(ClientID)) strSQL += "AND p.ClientID=" + Database.HandleQuote(ClientID);
                if (!string.IsNullOrEmpty(ItemParentID)) strSQL += "AND p.ItemParentID=" + Database.HandleQuote(ItemParentID);

                if (!string.IsNullOrEmpty(ItemAttributeID)) strSQL += "AND p.ItemAttributeID<>" + Database.HandleQuote(ItemAttributeID);
                return Database.HasRows(strSQL);
            }
        }

        public static ItemAttribute GetItemAttribute(string CompanyID, ItemAttributeFilter Filter)
        {
            List<ItemAttribute> objItemAttributes = null;
            ItemAttribute objReturn = null;

            try
            {
                objItemAttributes = GetItemAttributes(CompanyID, Filter);
                if (objItemAttributes != null && objItemAttributes.Count >= 1) objReturn = objItemAttributes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemAttributes = null;
            }
            return objReturn;
        }

        public static List<ItemAttribute> GetItemAttributes(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemAttributes(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemAttribute> GetItemAttributes(string CompanyID, ItemAttributeFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemAttributes(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemAttribute> GetItemAttributes(string CompanyID, ItemAttributeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemAttributes(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemAttribute> GetItemAttributes(string CompanyID, ItemAttributeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemAttribute> objReturn = null;
            ItemAttribute objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<ItemAttribute>();

                    strSQL = "SELECT i.* " +
                             "FROM ItemAttribute i (NOLOCK) " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.ClientID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ClientID, "i.ClientID");
                        if (Filter.ItemParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemParentID, "i.ItemParentID");
                        if (Filter.AttributeName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AttributeName, "i.AttributeName");
                    }

                    if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeName" : Utility.CustomSorting.GetSortExpression(typeof(ItemAttribute), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    else strSQL += " Order by ItemAttributeID";

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new ItemAttribute(objData.Tables[0].Rows[i]);
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
