using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using System.Security;
using System.ComponentModel.Design;

namespace ISLibrary
{
    public class ItemUnitType : BaseClass
    {
        public string ItemUnitTypeID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(ItemUnitTypeID); }
        }
        public string CompanyID { get; set; }
        public string ExternalID { get; set; }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<ItemUnit> mItemUnit = null;
        public List<ItemUnit> ItemUnits
        {
            get
            {
                ItemUnitFilter objFilter = null;

                try
                {
                    if (mItemUnit == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(ItemUnitTypeID))
                    {
                        objFilter = new ItemUnitFilter();
                        objFilter.ItemUnitTypeID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemUnitTypeID.SearchString = ItemUnitTypeID;
                        mItemUnit = ItemUnit.GetItemUnits(CompanyID, objFilter);
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
                return mItemUnit;
            }
            set
            {
                mItemUnit = value;
            }
        }

        public ItemUnitType()
        {
        }

        public ItemUnitType( string Id)
        {
            this.ItemUnitTypeID = Id;
            this.Load();
        }

        public ItemUnitType(DataRow objRow)
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
                         "FROM ItemUnitType (NOLOCK) " +
                         "WHERE ItemUnitTypeID=" + Database.HandleQuote(ItemUnitTypeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemUnitTypeID=" + ItemUnitTypeID + " is not found");
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

                if (objColumns.Contains("ItemUnitTypeID")) ItemUnitTypeID = Convert.ToString(objRow["ItemUnitTypeID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemUnitTypeID)) throw new Exception("Missing ItemUnitTypeID in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (!IsNew) throw new Exception("Create cannot be performed, AdjustmentID already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ItemID"] = ItemID;
                dicParam["Name"] = Name;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                ItemUnitTypeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemUnitType"), objConn, objTran)
                    .ToString();

                if (ItemUnits != null)
                {
                    foreach (ItemUnit objItemUnit in ItemUnits)
                    {
                        if (objItemUnit.IsNew)
                        {
                            objItemUnit.ItemUnitTypeID = ItemUnitTypeID;
                            objItemUnit.CompanyID = CompanyID;
                            objItemUnit.CreatedBy = CreatedBy;
                            objItemUnit.ParentKey = ItemUnitTypeID;
                            objItemUnit.ParentObject = "ItemUnitType";
                            objItemUnit.Create(objConn, objTran);
                        }
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
            }
            LogAuditData(enumActionType.Create);
            return true;
        }


        public static ItemUnitType GetItemUnitType(string CompanyID, ItemUnitTypeFilter Filter)
        {
            List<ItemUnitType> objAdjustments = null;
            ItemUnitType objReturn = null;

            try
            {
                objAdjustments = GetItemUnitTypes(CompanyID, Filter);
                if (objAdjustments != null && objAdjustments.Count >= 1) objReturn = objAdjustments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAdjustments = null;
            }

            return objReturn;
        }

        public static List<ItemUnitType> GetItemUnitTypes(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemUnitTypes(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemUnitType> GetItemUnitTypes(string CompanyID, ItemUnitTypeFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemUnitTypes(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemUnitType> GetItemUnitTypes(string CompanyID, ItemUnitTypeFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetItemUnitTypes(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemUnitType> GetItemUnitTypes(string CompanyID, ItemUnitTypeFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemUnitType> objReturn = null;
            ItemUnitType objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemUnitType>();

                strSQL = "SELECT * " +
                         "FROM ItemUnitType (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "ItemUnitTypeID"
                            : Utility.CustomSorting.GetSortExpression(typeof(ItemUnitType), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemUnitType(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }

                TotalRecord = objReturn.Count();
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