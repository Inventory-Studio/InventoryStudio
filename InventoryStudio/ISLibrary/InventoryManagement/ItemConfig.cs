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
    public class ItemConfig : BaseClass
    {
        public string ItemConfigID { get; set; } = string.Empty;
        public bool IsNew { get { return string.IsNullOrEmpty(ItemConfigID); } }
        public string CompanyID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ItemID { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        private List<ItemConfigLine>? mItemConfigLines = null;
        public List<ItemConfigLine>? ItemConfigLines
        {
            get
            {
                if (mItemConfigLines == null && !string.IsNullOrEmpty(ItemConfigID))
                {
                    ItemConfigLineFilter? objFilter = null;

                    try
                    {
                        objFilter = new ItemConfigLineFilter();
                        objFilter.ItemConfigID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemConfigID.SearchString = ItemConfigID;
                        mItemConfigLines = ItemConfigLine.GetItemConfigLines().ToList();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mItemConfigLines;
            }
        }

        public ItemConfig()
        {
        }

        public ItemConfig(string ItemConfigID)
        {
            this.ItemConfigID = ItemConfigID;
            Load();
        }

        public ItemConfig(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM ItemConfig (NOLOCK) " +
                         "WHERE ItemConfigID=" + Database.HandleQuote(ItemConfigID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemConfigID=" + ItemConfigID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objData = null;
            }
            base.Load();
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection? objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("ItemConfigID")) ItemConfigID = Convert.ToString(objRow["ItemConfigID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemConfigID)) throw new Exception("Missing ItemConfigID in the datarow");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objColumns = null;
            }
        }

        public override bool Create()
        {
            SqlConnection? objConn = null;
            SqlTransaction? objTran = null;

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
                throw new Exception(ex.Message);
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

            Hashtable? dicParam = null;

            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (ItemConfigLines == null || ItemConfigLines.Count == 0) throw new Exception("ItemConfigLines is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemConfigID already exists");

                dicParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["Name"] = Name;
                dicParam["ItemID"] = ItemID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;
                ItemConfigID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemConfig"), objConn, objTran).ToString();

                if (ItemConfigLines != null)
                {
                    foreach (ItemConfigLine objItemConfigLine in ItemConfigLines)
                    {
                        objItemConfigLine.ItemConfigID = ItemConfigID;
                        objItemConfigLine.Create(objConn, objTran);
                    }
                }

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dicParam = null;
            }
            return true;
        }

        public override bool Update()
        {
            SqlConnection? objConn = null;
            SqlTransaction? objTran = null;

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
                throw new Exception(ex.Message);
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
            Hashtable? dicParam = null;
            Hashtable? dicWParam = null;
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (ItemConfigLines == null || ItemConfigLines.Count == 0) throw new Exception("ItemConfigLines is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemConfigID is missing");

                dicParam = new Hashtable();
                dicWParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["Name"] = Name;
                dicParam["ItemID"] = ItemID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemConfigID"] = ItemConfigID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemConfig"), objConn, objTran);

                if (ItemConfigLines != null)
                {
                    foreach (ItemConfigLine objItemConfigLine in ItemConfigLines)
                    {
                        objItemConfigLine.Update(objConn, objTran);
                    }
                }

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
            SqlConnection? objConn = null;
            SqlTransaction? objTran = null;

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
                throw new Exception(ex.Message);
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

            Hashtable? dicDParam = null;

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ItemConfigID is missing");

                dicDParam = new Hashtable();
                dicDParam["ItemConfigID"] = ItemConfigID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemConfig"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        public static ItemConfig? GetItemConfig(ItemConfigFilter Filter)
        {
            List<ItemConfig>? objItemConfigs = null;
            ItemConfig? objReturn = null;

            try
            {
                objItemConfigs = GetItemConfigs(Filter);
                if (objItemConfigs != null && objItemConfigs.Count >= 1) objReturn = objItemConfigs[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objItemConfigs = null;
            }
            return objReturn;
        }

        public static List<ItemConfig> GetItemConfigs()
        {
            int intTotalCount = 0;
            return GetItemConfigs(null, null, null, out intTotalCount);
        }

        public static List<ItemConfig> GetItemConfigs(ItemConfigFilter? Filter)
        {
            int intTotalCount = 0;
            return GetItemConfigs(Filter, null, null, out intTotalCount);
        }

        public static List<ItemConfig> GetItemConfigs(ItemConfigFilter? Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemConfigs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemConfig> GetItemConfigs(ItemConfigFilter? Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemConfig>? objReturn = null;
            ItemConfig? objNew = null;
            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemConfig>();

                strSQL = "SELECT s.* " +
                         "FROM ItemConfig (NOLOCK) s " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CompanyID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "CompanyID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemConfigID" : Utility.CustomSorting.GetSortExpression(typeof(ItemConfig), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemConfig(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }
    }
}
