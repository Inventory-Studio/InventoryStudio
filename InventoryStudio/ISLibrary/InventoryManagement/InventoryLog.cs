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
    public class InventoryLog : BaseClass
    {
        public string InventoryLogID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(InventoryLogID); }
        }
        public string CompanyID { get; set; }
        public string ItemID { get; set; }
        public string ChangeType { get; set; }
        public decimal ChangeQuantity { get; set; }
        public string ParentObjectID { get; set; }
        public string BinID { get; set; }
        public string InventoryNumber { get; set; }
        public string InventoryID { get; set; }
        public string Memo { get; set; }
        public DateTime CreatedOn { get; set; }


        public InventoryLog()
        {
        }

        public InventoryLog( string Id)
        {
            this.InventoryLogID = Id;
            this.Load();
        }

        public InventoryLog(DataRow objRow)
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
                         "FROM InventoryLog (NOLOCK) " +
                         "WHERE InventoryLogID=" + Database.HandleQuote(InventoryLogID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InventoryLogID=" + InventoryLogID + " is not found");
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

                if (objColumns.Contains("InventoryLogID")) InventoryLogID = Convert.ToString(objRow["InventoryLogID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
          
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InventoryLogID)) throw new Exception("Missing AdjustmentID in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (!IsNew) throw new Exception("Create cannot be performed, InventoryLogID already exists");
               

                dicParam["CompanyID"] = CompanyID;
                dicParam["ItemID"] = ItemID;
                dicParam["ChangeType"] = ChangeType;
                dicParam["ChangeQuantity"] = ChangeQuantity;
                dicParam["ParentObjectID"] = ParentObjectID;
                dicParam["BinID"] = BinID;
                dicParam["InventoryNumber"] = InventoryNumber;
                dicParam["InventoryID"] = InventoryID;
                dicParam["Memo"] = Memo;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                InventoryLogID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InventoryLog"), objConn, objTran)
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
            return true;
        }

       

       

      

    


    }
}