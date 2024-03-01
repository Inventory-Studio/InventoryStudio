﻿using System;
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
    public class ItemUnit : BaseClass
    {
        public string ItemUnitID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(ItemUnitID); }
        }
        public string CompanyID { get; set; }
        public string ExternalID { get; set; }
        public string ItemUnitTypeID { get; set; }
        public string Name { get; set; }
        public bool IsBaseUnit { get; set; }
        public bool IsActive { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreatedOn { get; set; }


        public ItemUnit()
        {
        }

        public ItemUnit( string Id)
        {
            this.ItemUnitID = Id;
            this.Load();
        }

        public ItemUnit(DataRow objRow)
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
                         "FROM ItemUnit (NOLOCK) " +
                         "WHERE ItemUnitID=" + Database.HandleQuote(ItemUnitID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemUnitID=" + ItemUnitID + " is not found");
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

                if (objColumns.Contains("ItemUnitID")) ItemUnitID = Convert.ToString(objRow["ItemUnitID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("ItemUnitTypeID")) ItemUnitTypeID = Convert.ToString(objRow["ItemUnitTypeID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("IsBaseUnit")) IsBaseUnit = Convert.ToBoolean(objRow["IsBaseUnit"]);
                if (objColumns.Contains("IsActive")) IsActive = Convert.ToBoolean(objRow["IsActive"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToDecimal(objRow["Quantity"]);
              

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemUnitID)) throw new Exception("Missing AdjustmentID in the datarow");
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
                dicParam["ItemUnitTypeID"] = ItemUnitTypeID;
                dicParam["Name"] = Name;
                dicParam["IsBaseUnit"] = IsBaseUnit;
                dicParam["IsActive"] = IsActive;
                dicParam["Quantity"] = Quantity;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                ItemUnitID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemUnit"), objConn, objTran)
                    .ToString();


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


        public static ItemUnit GetItemUnit(string CompanyID, ItemUnitFilter Filter)
        {
            List<ItemUnit> objAdjustments = null;
            ItemUnit objReturn = null;

            try
            {
                objAdjustments = GetItemUnits(CompanyID, Filter);
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

        public static List<ItemUnit> GetItemUnits(string CompanyID)
        {
            int intTotalCount = 0;
            return GetItemUnits(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<ItemUnit> GetItemUnits(string CompanyID, ItemUnitFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemUnits(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<ItemUnit> GetItemUnits(string CompanyID, ItemUnitFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetItemUnits(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemUnit> GetItemUnits(string CompanyID, ItemUnitFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemUnit> objReturn = null;
            ItemUnit objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemUnit>();

                strSQL = "SELECT * " +
                         "FROM ItemUnit (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.ItemUnitTypeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemUnitTypeID, "ItemUnitTypeID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "ItemUnitID"
                            : Utility.CustomSorting.GetSortExpression(typeof(ItemUnit), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemUnit(objData.Tables[0].Rows[i]);
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