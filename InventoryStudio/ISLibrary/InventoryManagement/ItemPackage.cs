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
    public class ItemPackage : BaseClass
    {
        public string ItemPackageID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemPackageID); } }
        public string CompanyID { get; private set; }
        public string ItemID { get; set; }
        public string ItemVariationID { get; set; }
        public string ShippingCarrierID { get; set; }
        public string ShippingPackageID { get; set; }
        public int MaxQuantity { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        //private Shipping.ShippingPackage mShippingPackage = null;
        //public Shipping.ShippingPackage ShippingPackage
        //{
        //    get
        //    {
        //        if (mShippingPackage == null && !string.IsNullOrEmpty(ShippingPackageID))
        //        {
        //            mShippingPackage = new Shipping.ShippingPackage(ShippingPackageID);
        //        }
        //        return mShippingPackage;
        //    }
        //}
        public ItemPackage()
        {
        }
        public ItemPackage(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public ItemPackage(string CompanyID, string ItemPackageID)
        {
            this.CompanyID = CompanyID;
            this.ItemPackageID = ItemPackageID;
            Load();
        }

        public ItemPackage(DataRow objRow)
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
                         "FROM ItemPackage (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND ItemPackageID=" + Database.HandleQuote(ItemPackageID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemPackageID=" + ItemPackageID + " is not found");
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

                if (objColumns.Contains("ItemPackageID")) ItemPackageID = Convert.ToString(objRow["ItemPackageID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ItemVariationID")) ItemVariationID = Convert.ToString(objRow["ItemVariationID"]);
                if (objColumns.Contains("ShippingCarrierID")) ShippingCarrierID = Convert.ToString(objRow["ShippingCarrierID"]);
                if (objColumns.Contains("ShippingPackageID")) ShippingPackageID = Convert.ToString(objRow["ShippingPackageID"]);
                if (objColumns.Contains("MaxQuantity")) MaxQuantity = Convert.ToInt32(objRow["MaxQuantity"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemPackageID)) throw new Exception("Missing ItemPackageID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
            base.Load();
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
                if (string.IsNullOrEmpty(ShippingCarrierID)) throw new Exception("ShippingCarrierID is required");
                if (string.IsNullOrEmpty(ShippingPackageID)) throw new Exception("ShippingPackageID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (MaxQuantity < 1) throw new Exception("MaxQuantity must be at least 1 or more");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemPackageID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ItemVariationID"] = ItemVariationID;
                dicParam["ShippingCarrierID"] = ShippingCarrierID;
                dicParam["ShippingPackageID"] = ShippingPackageID;
                dicParam["MaxQuantity"] = MaxQuantity;
                dicParam["CreatedBy"] = CreatedBy;
                ItemPackageID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemPackage"), objConn, objTran).ToString();
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
            LogAuditData(enumActionType.Create);
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
                if (string.IsNullOrEmpty(ShippingCarrierID)) throw new Exception("ShippingCarrierID is required");
                if (string.IsNullOrEmpty(ShippingPackageID)) throw new Exception("ShippingPackageID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (MaxQuantity < 1) throw new Exception("MaxQuantity must be at least 1 or more");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ItemVariationID"] = ItemVariationID;
                dicParam["ShippingCarrierID"] = ShippingCarrierID;
                dicParam["ShippingPackageID"] = ShippingPackageID;
                dicParam["MaxQuantity"] = MaxQuantity;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemPackageID"] = ItemPackageID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemPackage"), objConn, objTran);
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
            
            LogAuditData(enumActionType.Update);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemPackageID is missing");

                dicDParam["ItemPackageID"] = ItemPackageID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemPackage"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            LogAuditData(enumActionType.Delete);
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM ItemPackage (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.ItemID=" + Database.HandleQuote(ItemID) +
                     "AND p.ShippingPackageID=" + Database.HandleQuote(ShippingPackageID);

            if (!string.IsNullOrEmpty(ItemPackageID)) strSQL += "AND p.ItemPackageID<>" + Database.HandleQuote(ItemPackageID);
            return Database.HasRows(strSQL);
        }

        public static ItemPackage GetItemPackage(string CompanyID, ItemPackageFilter Filter)
        {
            List<ItemPackage> objItemPackages = null;
            ItemPackage objReturn = null;

            try
            {
                objItemPackages = GetItemPackages(CompanyID, Filter);
                if (objItemPackages != null && objItemPackages.Count >= 1) objReturn = objItemPackages[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemPackages = null;
            }
            return objReturn;
        }

        public static List<ItemPackage> GetItemPackages(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemPackages(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemPackage> GetItemPackages(string CompanyID, ItemPackageFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemPackages(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemPackage> GetItemPackages(string CompanyID, ItemPackageFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemPackages(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemPackage> GetItemPackages(string CompanyID, ItemPackageFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemPackage> objReturn = null;
            ItemPackage objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemPackage>();

                strSQL = "SELECT * " +
                         "FROM ItemPackage (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.ItemVariationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemVariationID, "ItemVariationID");
                    if (Filter.ShippingCarrierID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShippingCarrierID, "ShippingCarrierID");
                    if (Filter.ShippingPackageID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShippingPackageID, "ShippingPackageID");
                    if (Filter.MaxQuantity != null) strSQL += Database.Filter.NumericSearch.GetSQLQuery(Filter.MaxQuantity, "MaxQuantity");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemPackageID" : Utility.CustomSorting.GetSortExpression(typeof(ItemPackage), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemPackage(objData.Tables[0].Rows[i]);
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
