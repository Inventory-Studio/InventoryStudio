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
    public class InventoryLine : BaseClass
    {
        public string InventoryLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(InventoryLineID); } }
        public string CompanyID { get; set; }
        public string InventoryID { get; set; }
        public string ParentInventoryLineID { get; set; }
        public string ItemID { get; set; }
        public int KitQuantity { get; set; }
        public int OnHand { get; set; }
        public int Available { get; set; }
        public string SerialLotNumber { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public InventoryLine()
        {
        }

        public InventoryLine(string CompanyID)
        {
            this.CompanyID = CompanyID;
        }

        public InventoryLine(string CompanyID, string InventoryLineID)
        {
            this.CompanyID = CompanyID;
            this.InventoryLineID = InventoryLineID;
            Load();
        }

        public InventoryLine(DataRow objRow)
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
                strSQL = "SELECT * " +
                            "FROM InventoryLine (NOLOCK) " +
                            "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                            "AND InventoryLineID=" + Database.HandleQuote(InventoryLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InventoryLineID=" + InventoryLineID + " is not found");
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

                if (objColumns.Contains("InventoryLineID")) InventoryLineID = Convert.ToString(objRow["InventoryLineID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("InventoryID")) InventoryID = Convert.ToString(objRow["InventoryID"]);
                if (objColumns.Contains("ParentInventoryLineID")) ParentInventoryLineID = Convert.ToString(objRow["ParentInventoryLineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("KitQuantity")) KitQuantity = Convert.ToInt32(objRow["KitQuantity"]);
                if (objColumns.Contains("OnHand")) OnHand = Convert.ToInt32(objRow["OnHand"]);
                if (objColumns.Contains("Available")) Available = Convert.ToInt32(objRow["Available"]);
                if (objColumns.Contains("SerialLotNumber")) SerialLotNumber = Convert.ToString(objRow["SerialLotNumber"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InventoryLineID)) throw new Exception("Missing InventoryLineID in the datarow");
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
                if (string.IsNullOrEmpty(InventoryID)) throw new Exception("InventoryID is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentInventoryLineID"] = ParentInventoryLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["KitQuantity"] = KitQuantity;
                dicParam["OnHand"] = OnHand;
                dicParam["Available"] = Available;
                dicParam["SerialLotNumber"] = SerialLotNumber;
                dicParam["CreatedBy"] = CreatedBy;
                InventoryLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InventoryLine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(InventoryID)) throw new Exception("InventoryID is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, InventoryLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentInventoryLineID"] = ParentInventoryLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["KitQuantity"] = KitQuantity;
                dicParam["OnHand"] = OnHand;
                dicParam["Available"] = Available;
                dicParam["SerialLotNumber"] = SerialLotNumber;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["InventoryLineID"] = InventoryLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "InventoryLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, InventoryLineID is missing");

                dicDParam["InventoryLineID"] = InventoryLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "InventoryLine"), objConn, objTran);
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
                        "FROM InventoryLine (NOLOCK) p " +
                        "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                        "AND p.InventoryID=" + Database.HandleQuote(InventoryID);

            if (!string.IsNullOrEmpty(InventoryLineID)) strSQL += "AND p.InventoryLineID<>" + Database.HandleQuote(InventoryLineID);
            return Database.HasRows(strSQL);
        }

        public static InventoryLine GetInventoryLine(string CompanyID, InventoryLineFilter Filter)
        {
            List<InventoryLine> objItemAttributes = null;
            InventoryLine objReturn = null;

            try
            {
                objItemAttributes = GetInventoryLines(CompanyID, Filter);
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

        public static List<InventoryLine> GetInventoryLines(string CompanyID)
        {
            int intTotalCount = 0;
            return GetInventoryLines(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<InventoryLine> GetInventoryLines(string CompanyID, InventoryLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetInventoryLines(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<InventoryLine> GetInventoryLines(string CompanyID, InventoryLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInventoryLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<InventoryLine> GetInventoryLines(string CompanyID, InventoryLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<InventoryLine> objReturn = null;
            InventoryLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    objReturn = new List<InventoryLine>();

                    strSQL = "SELECT i.* " +
                                "FROM InventoryLine i (NOLOCK) " +
                                "WHERE i.CompanyID=" + Database.HandleQuote(CompanyID);

                    if (Filter != null)
                    {
                        if (Filter.InventoryLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InventoryLineID, "i.InventoryLineID");
                        if (Filter.InventoryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InventoryID, "i.InventoryID");
                        if (Filter.CompanyID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyID, "i.CompanyID");
                        if (Filter.ParentInventoryLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentInventoryLineID, "i.ParentInventoryLineID");
                    }

                    if (PageSize != null && PageNumber != null)
                        strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeValueName" : Utility.CustomSorting.GetSortExpression(typeof(InventoryLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    else
                        strSQL += "ORDER BY AttributeValueName ";
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new InventoryLine(objData.Tables[0].Rows[i]);
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
