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
    public class ItemInventoryStatus : BaseClass
    {
        public string ItemInventoryStatusID { get; set; } = string.Empty;
        public bool IsNew { get { return string.IsNullOrEmpty(ItemInventoryStatusID); } }
        public string CompanyID { get; set; } = string.Empty;
        public string ItemID { get; set; } = string.Empty;
        public string InventoryStatusID { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        public ItemInventoryStatus()
        {
        }

        public ItemInventoryStatus(string ItemInventoryStatusID)
        {
            this.ItemInventoryStatusID = ItemInventoryStatusID;
            Load();
        }

        public ItemInventoryStatus(DataRow objRow)
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
                         "FROM ItemInventoryStatus (NOLOCK) " +
                         "WHERE ItemInventoryStatusID=" + Database.HandleQuote(ItemInventoryStatusID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemInventoryStatusID=" + ItemInventoryStatusID + " is not found");
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

                if (objColumns.Contains("ItemInventoryStatusID")) ItemInventoryStatusID = Convert.ToString(objRow["ItemInventoryStatusID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("InventoryStatusID")) InventoryStatusID = Convert.ToString(objRow["InventoryStatusID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemInventoryStatusID)) throw new Exception("Missing ItemInventoryStatusID in the datarow");
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
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(InventoryStatusID)) throw new Exception("InventoryStatusID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemInventoryStatusID already exists");

                dicParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["InventoryStatusID"] = InventoryStatusID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;
                ItemInventoryStatusID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemInventoryStatus"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(InventoryStatusID)) throw new Exception("InventoryStatusID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemInventoryStatusID is missing");

                dicParam = new Hashtable();
                dicWParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["InventoryStatusID"] = InventoryStatusID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemInventoryStatusID"] = ItemInventoryStatusID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemInventoryStatus"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemInventoryStatusID is missing");

                dicDParam = new Hashtable();
                dicDParam["ItemInventoryStatusID"] = ItemInventoryStatusID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemInventoryStatus"), objConn, objTran);
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

        public static ItemInventoryStatus? GetItemInventoryStatus(ItemInventoryStatusFilter Filter)
        {
            List<ItemInventoryStatus>? objItemInventoryStatuss = null;
            ItemInventoryStatus? objReturn = null;

            try
            {
                objItemInventoryStatuss = GetItemInventoryStatuss(Filter);
                if (objItemInventoryStatuss != null && objItemInventoryStatuss.Count >= 1) objReturn = objItemInventoryStatuss[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objItemInventoryStatuss = null;
            }
            return objReturn;
        }

        public static List<ItemInventoryStatus> GetItemInventoryStatuss()
        {
            int intTotalCount = 0;
            return GetItemInventoryStatuss(null, null, null, out intTotalCount);
        }

        public static List<ItemInventoryStatus> GetItemInventoryStatuss(ItemInventoryStatusFilter? Filter)
        {
            int intTotalCount = 0;
            return GetItemInventoryStatuss(Filter, null, null, out intTotalCount);
        }

        public static List<ItemInventoryStatus> GetItemInventoryStatuss(ItemInventoryStatusFilter? Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemInventoryStatuss(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemInventoryStatus> GetItemInventoryStatuss(ItemInventoryStatusFilter? Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemInventoryStatus>? objReturn = null;
            ItemInventoryStatus? objNew = null;
            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemInventoryStatus>();

                strSQL = "SELECT s.* " +
                         "FROM ItemInventoryStatus (NOLOCK) s " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CompanyID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "CompanyID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.InventoryStatusID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InventoryStatusID, "InventoryStatusID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemInventoryStatusID" : Utility.CustomSorting.GetSortExpression(typeof(ItemInventoryStatus), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemInventoryStatus(objData.Tables[0].Rows[i]);
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
