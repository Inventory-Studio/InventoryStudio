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
    public class ItemBarcode : BaseClass
    {
        public string ItemBarcodeID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemBarcodeID); } }
        public string CompanyID { get; set; }
        public string ItemID { get; set; }
        public string Barcode { get; set; }
        public string Type { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public ItemBarcode()
        {
        }

        public ItemBarcode(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemBarcode(string CompanyID, string ItemBarcodeID)
        {
            this.CompanyID = CompanyID;
            this.ItemBarcodeID = ItemBarcodeID;
            Load();
        }

        public ItemBarcode(DataRow objRow)
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
                         "FROM ItemBarcode (NOLOCK) " +
                         "WHERE ItemBarcodeID=" + Database.HandleQuote(ItemBarcodeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemBarcodeID=" + ItemBarcodeID + " is not found");
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

                if (objColumns.Contains("ItemBarcodeID")) ItemBarcodeID = Convert.ToString(objRow["ItemBarcodeID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Barcode")) Barcode = Convert.ToString(objRow["Barcode"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemBarcodeID)) throw new Exception("Missing ItemBarcodeID in the datarow");
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
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemBarcodeID already exists");
                if (ObjectAlreadyExists()) throw new Exception(string.Format("Item Barcode {0} already exists", Barcode));

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["Barcode"] = Barcode;
                dicParam["Type"] = Type;
                dicParam["CreatedBy"] = CreatedBy;
                ItemBarcodeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemBarcode"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception(string.Format("Item Barcode {0} already exists", Barcode));

                dicParam["Barcode"] = Barcode;
                dicParam["Type"] = Type;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemBarcodeID"] = ItemBarcodeID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemBarcode"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemBarcodeID is missing");

                dicDParam["ItemBarcodeID"] = ItemBarcodeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemBarcode"), objConn, objTran);
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
                     "FROM ItemBarcode (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID);

            if (!string.IsNullOrEmpty(ItemBarcodeID)) strSQL += "AND p.ItemBarcodeID != " + Database.HandleQuote(ItemBarcodeID);
            if (!string.IsNullOrEmpty(Barcode)) strSQL += "AND p.Barcode = " + Database.HandleQuote(Barcode);

            return Database.HasRows(strSQL);
        }

        public static ItemBarcode GetItemBarcode(string CompanyID, ItemBarcodeFilter Filter)
        {
            List<ItemBarcode> objItemBarcodes = null;
            ItemBarcode objReturn = null;

            try
            {
                objItemBarcodes = GetItemBarcodes(CompanyID, Filter);
                if (objItemBarcodes != null && objItemBarcodes.Count >= 1) objReturn = objItemBarcodes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemBarcodes = null;
            }
            return objReturn;
        }

        public static List<ItemBarcode> GetItemBarcodes(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemBarcodes(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemBarcode> GetItemBarcodes(string CompanyID, ItemBarcodeFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemBarcodes(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemBarcode> GetItemBarcodes(string CompanyID, ItemBarcodeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemBarcodes(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemBarcode> GetItemBarcodes(string CompanyID, ItemBarcodeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemBarcode> objReturn = null;
            ItemBarcode objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemBarcode>();

                strSQL = "SELECT * " +
                         "FROM ItemBarcode (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemBarcodeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemBarcodeID, "ItemBarcodeID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.Barcode != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Barcode, "Barcode");
                    if (Filter.Type != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Type, "Type");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemBarcodeID" : Utility.CustomSorting.GetSortExpression(typeof(ItemBarcode), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemBarcode(objData.Tables[0].Rows[i]);
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
