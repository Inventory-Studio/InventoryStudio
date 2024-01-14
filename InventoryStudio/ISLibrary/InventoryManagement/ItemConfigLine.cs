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
using System.Runtime.Serialization;

namespace ISLibrary
{
    public class ItemConfigLine : BaseClass
    {
        public string ItemConfigLineID { get; set; } = string.Empty;
        public bool IsNew { get { return string.IsNullOrEmpty(ItemConfigLineID); } }
        public string CompanyID { get; set; } = string.Empty;
        public string ItemConfigID { get; set; } = string.Empty;
        public string ItemComponentID { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        public ItemConfigLine()
        {
        }

        public ItemConfigLine(string ItemConfigLineID)
        {
            this.ItemConfigLineID = ItemConfigLineID;
            Load();
        }

        public ItemConfigLine(DataRow objRow)
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
                         "FROM ItemConfigLine (NOLOCK) " +
                         "WHERE ItemConfigLineID=" + Database.HandleQuote(ItemConfigLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemConfigLineID=" + ItemConfigLineID + " is not found");
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

                if (objColumns.Contains("ItemConfigLineID")) ItemConfigLineID = Convert.ToString(objRow["ItemConfigLineID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemConfigID")) ItemConfigID = Convert.ToString(objRow["ItemConfigID"]);
                if (objColumns.Contains("ItemComponentID")) ItemComponentID = Convert.ToString(objRow["ItemComponentID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToDouble(objRow["Quantity"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemConfigLineID)) throw new Exception("Missing ItemConfigLineID in the datarow");
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
                if (string.IsNullOrEmpty(ItemConfigID)) throw new Exception("ItemConfigID is required");
                if (string.IsNullOrEmpty(ItemComponentID)) throw new Exception("ItemComponentID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemConfigLineID already exists");

                dicParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemConfigID"] = ItemConfigID;
                dicParam["ItemComponentID"] = ItemComponentID;
                dicParam["Quantity"] = Quantity;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;
                ItemConfigLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemConfigLine"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ItemConfigID)) throw new Exception("ItemConfigID is required");
                if (string.IsNullOrEmpty(ItemComponentID)) throw new Exception("ItemComponentID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemConfigLineID is missing");

                dicParam = new Hashtable();
                dicWParam = new Hashtable();
                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemConfigID"] = ItemConfigID;
                dicParam["ItemComponentID"] = ItemComponentID;
                dicParam["Quantity"] = Quantity;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemConfigLineID"] = ItemConfigLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemConfigLine"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemConfigLineID is missing");

                dicDParam = new Hashtable();
                dicDParam["ItemConfigLineID"] = ItemConfigLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemConfigLine"), objConn, objTran);
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

        public static ItemConfigLine? GetItemConfigLine(ItemConfigLineFilter Filter)
        {
            List<ItemConfigLine>? objItemConfigLines = null;
            ItemConfigLine? objReturn = null;

            try
            {
                objItemConfigLines = GetItemConfigLines(Filter);
                if (objItemConfigLines != null && objItemConfigLines.Count >= 1) objReturn = objItemConfigLines[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objItemConfigLines = null;
            }
            return objReturn;
        }

        public static List<ItemConfigLine> GetItemConfigLines()
        {
            int intTotalCount = 0;
            return GetItemConfigLines(null, null, null, out intTotalCount);
        }

        public static List<ItemConfigLine> GetItemConfigLines(ItemConfigLineFilter? Filter)
        {
            int intTotalCount = 0;
            return GetItemConfigLines(Filter, null, null, out intTotalCount);
        }

        public static List<ItemConfigLine> GetItemConfigLines(ItemConfigLineFilter? Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemConfigLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemConfigLine> GetItemConfigLines(ItemConfigLineFilter? Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemConfigLine>? objReturn = null;
            ItemConfigLine? objNew = null;
            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemConfigLine>();

                strSQL = "SELECT s.* " +
                         "FROM ItemConfigLine (NOLOCK) s " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CompanyID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "CompanyID");
                    if (Filter.ItemConfigID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemConfigID, "ItemConfigID");
                    if (Filter.ItemComponentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemComponentID, "ItemComponentID");
                    if (Filter.Quantity != null) strSQL += Database.Filter.NumericSearch.GetSQLQuery(Filter.Quantity, "Quantity");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemConfigLineID" : Utility.CustomSorting.GetSortExpression(typeof(ItemConfigLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemConfigLine(objData.Tables[0].Rows[i]);
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
