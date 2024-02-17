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
    public class ItemAttributeValue : BaseClass
    {
        public string ItemAttributeValueID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemAttributeValueID); } }
        public string CompanyID { get; set; }
        public string ItemAttributeID { get; set; }
        public string AttributeValueName { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        //private List<ItemConfigurationSetup> mItemConfigurationSetups = null;
        //public List<ItemConfigurationSetup> ItemConfigurationSetups
        //{
        //    get
        //    {
        //        ItemConfigurationSetupFilter objFilter = null;

        //        try
        //        {
        //            if (mItemConfigurationSetups == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemAttributeValueID))
        //            {
        //                objFilter = new ItemConfigurationSetupFilter();
        //                objFilter.ItemAttributeValueID = ItemAttributeValueID;
        //                mItemConfigurationSetups = ItemConfigurationSetup.GetItemConfigurationSetups(CompanyID, objFilter);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            objFilter = null;
        //        }
        //        return mItemConfigurationSetups;
        //    }
        //}

        public ItemAttributeValue()
        {
        }

        public ItemAttributeValue(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemAttributeValue(string CompanyID, string ItemAttributeValueID)
        {
            this.CompanyID = CompanyID;
            this.ItemAttributeValueID = ItemAttributeValueID;
            Load();
        }

        public ItemAttributeValue(DataRow objRow)
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
                         "FROM ItemAttributeValue (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND ItemAttributeValueID=" + Database.HandleQuote(ItemAttributeValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemAttributeValueID=" + ItemAttributeValueID + " is not found");
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

                if (objColumns.Contains("ItemAttributeValueID")) ItemAttributeValueID = Convert.ToString(objRow["ItemAttributeValueID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemAttributeID")) ItemAttributeID = Convert.ToString(objRow["ItemAttributeID"]);
                if (objColumns.Contains("AttributeValueName")) AttributeValueName = Convert.ToString(objRow["AttributeValueName"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemAttributeID)) throw new Exception("Missing ItemAttributeValueID in the datarow");
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
                if (string.IsNullOrEmpty(ItemAttributeID)) throw new Exception("ItemAttributeID is required");
                if (string.IsNullOrEmpty(AttributeValueName)) throw new Exception("Attribute value is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemAttributeID"] = ItemAttributeID;
                dicParam["AttributeValueName"] = AttributeValueName;
                dicParam["CreatedBy"] = CreatedBy;
                ItemAttributeValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemAttributeValue"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(ItemAttributeID)) throw new Exception("ItemAttributeID is required");
                if (string.IsNullOrEmpty(AttributeValueName)) throw new Exception("Attribute value is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemAttributeValueID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemAttributeID"] = ItemAttributeID;
                dicParam["AttributeValueName"] = AttributeValueName;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemAttributeValueID"] = ItemAttributeValueID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemAttributeValue"), objConn, objTran);
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
            ItemMatrixValueFilter objFilter = null;
            List<ItemMatrixValue> objItemMatrixValues = null;

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ItemAttributeValueID is missing");

                objFilter = new ItemMatrixValueFilter();
                objFilter.ItemAttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ItemAttributeValueID.SearchString = ItemAttributeValueID;
                objItemMatrixValues = ItemMatrixValue.GetItemMatrixValues(CompanyID, objFilter);
                foreach (ItemMatrixValue objItemMatrixValue in objItemMatrixValues)
                {
                        objItemMatrixValue.Delete(objConn, objTran);                    
                }

                dicDParam["ItemAttributeValueID"] = ItemAttributeValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemAttributeValue"), objConn, objTran);
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
                     "FROM ItemAttributeValue (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemAttributeID=" + Database.HandleQuote(ItemAttributeID) +
                     "AND p.AttributeValueName=" + Database.HandleQuote(AttributeValueName);

            if (!string.IsNullOrEmpty(ItemAttributeValueID)) strSQL += "AND p.ItemAttributeValueID<>" + Database.HandleQuote(ItemAttributeValueID);
            return Database.HasRows(strSQL);
        }

        public static ItemAttributeValue GetItemAttributeValue(string CompanyID, ItemAttributeValueFilter Filter)
        {
            List<ItemAttributeValue> objItemAttributes = null;
            ItemAttributeValue objReturn = null;

            try
            {
                objItemAttributes = GetItemAttributeValues(CompanyID, Filter);
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

        public static List<ItemAttributeValue> GetItemAttributeValues(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemAttributeValues(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemAttributeValue> GetItemAttributeValues(string CompanyID, ItemAttributeValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemAttributeValues(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemAttributeValue> GetItemAttributeValues(string CompanyID, ItemAttributeValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemAttributeValues(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemAttributeValue> GetItemAttributeValues(string CompanyID, ItemAttributeValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemAttributeValue> objReturn = null;
            ItemAttributeValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<ItemAttributeValue>();

                    strSQL = "SELECT i.* " +
                             "FROM ItemAttributeValue i (NOLOCK) " +
                             "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.ItemAttributeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemAttributeID, "i.ItemAttributeID");
                        if (Filter.AttributeValueName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AttributeValueName, "i.AttributeValueName");
                    }

                    if (PageSize != null && PageNumber != null)
                        strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeValueName" : Utility.CustomSorting.GetSortExpression(typeof(ItemAttributeValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    else
                        strSQL += " ORDER BY ItemAttributeValueID ";
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new ItemAttributeValue(objData.Tables[0].Rows[i]);
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
