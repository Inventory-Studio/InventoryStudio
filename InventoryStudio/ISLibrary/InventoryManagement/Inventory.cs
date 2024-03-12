using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;

namespace ISLibrary
{
    public class Inventory : BaseClass
    {
        public string InventoryID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(InventoryID); }
        }
        public string ItemID { get; set; }
        public string LocationID { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal QtyOnOrder { get; set; }
        public decimal QtyCommitted { get; set; }
        public decimal QtyAvailable { get; set; }
        public decimal QtyBackOrdered { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        public Inventory()
        {
        }

        public Inventory( string Id)
        {
            this.InventoryID = Id;
            this.Load();
        }

        public Inventory(DataRow objRow)
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
                         "FROM Inventory (NOLOCK) " +
                         "WHERE InventoryID=" + Database.HandleQuote(InventoryID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InventoryID=" + InventoryID + " is not found");
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

                if (objColumns.Contains("InventoryID")) InventoryID = Convert.ToString(objRow["InventoryID"]);
              
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("QtyOnHand")) QtyOnHand = Convert.ToDecimal(objRow["QtyOnHand"]);
                if (objColumns.Contains("QtyOnOrder")) QtyOnOrder = Convert.ToDecimal(objRow["QtyOnOrder"]);
                if (objColumns.Contains("QtyCommitted")) QtyCommitted = Convert.ToDecimal(objRow["QtyCommitted"]);
                if (objColumns.Contains("QtyAvailable")) QtyAvailable = Convert.ToDecimal(objRow["QtyAvailable"]);
                if (objColumns.Contains("QtyBackOrdered")) QtyBackOrdered = Convert.ToDecimal(objRow["QtyBackOrdered"]);
      

                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InventoryID)) throw new Exception("Missing AdjustmentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, InventoryID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["LocationID"] = LocationID;
                dicParam["QtyOnHand"] = QtyOnHand;
                dicParam["QtyOnOrder"] = QtyOnOrder;
                dicParam["QtyCommitted"] = QtyCommitted;
                dicParam["QtyAvailable"] = QtyAvailable;
                dicParam["QtyBackOrdered"] = QtyBackOrdered;

                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                InventoryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Inventory"), objConn, objTran)
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
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");


                dicParam["QtyOnHand"] = QtyOnHand;
                dicParam["QtyOnOrder"] = QtyOnOrder;
                dicParam["QtyCommitted"] = QtyCommitted;
                dicParam["QtyAvailable"] = QtyAvailable;
                dicParam["QtyBackOrdered"] = QtyBackOrdered;

                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["InventoryID"] = InventoryID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Inventory"), objConn, objTran);

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

  
        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM Inventory (NOLOCK) p " +
                     "WHERE (p.LocationID=" + Database.HandleQuote(LocationID);
        
            strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID) + ")";
           

            return Database.HasRows(strSQL);
        }



        /** used for InventoryDetail to get the unique Inventory **/
        public static Inventory GetInventory( InventoryFilter Filter)
        {
            Inventory objReturn = null;
            Inventory objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {


                strSQL = "SELECT  TOP 1 * " +
                         "FROM Inventory (NOLOCK) " +
                         "WHERE 1=1 " ;
                strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "LocationID");
                strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");



                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Inventory(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn = objNew;
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