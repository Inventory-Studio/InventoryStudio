﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CLRFramework;
using System.ComponentModel.Design;
using System.Security.AccessControl;

namespace ISLibrary
{
    public class AuditData : BaseClass
    {
        public string AuditDataID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AuditDataID); } }
        public string ObjectID { get; set; }
        public string Type { get; set; }
        public string ObjectName { get; set; }
        public string ChangedValue { get; set; }
        public string CreatedBy { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedOn { get; private set; }

        public AuditData()
        {
        }

        public AuditData(string AuditDataID)
        {
            this.AuditDataID = AuditDataID;
        }


        public AuditData(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT i.* " +
                         "FROM AuditData i (NOLOCK) ";

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AuditData is not found");
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
                if (objColumns.Contains("AuditDataID")) AuditDataID = Convert.ToString(objRow["AuditDataID"]);
                if (objColumns.Contains("ObjectID")) ObjectID = Convert.ToString(objRow["ObjectID"]);
                if (objColumns.Contains("ObjectName")) ObjectName = Convert.ToString(objRow["ObjectName"]);
                if (objColumns.Contains("ChangedValue")) ChangedValue = Convert.ToString(objRow["ChangedValue"]);
                if (objColumns.Contains("UserName")) UserName = Convert.ToString(objRow["UserName"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);

                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AuditDataID)) throw new Exception("Missing AuditDataID in the datarow");
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
            ItemParent objItemParent = null;

            try
            {
                dicParam["ObjectID"] = ObjectID;
                dicParam["ObjectName"] = ObjectName;
                dicParam["Type"] = Type;
                dicParam["ParentKey"] = ParentKey;
                dicParam["ParentObject"] = ParentObject;
                dicParam["ChangedValue"] = ChangedValue;
                dicParam["CreatedBy"] = CreatedBy;

                AuditDataID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AuditData"), objConn, objTran).ToString();



                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                objItemParent = null;
            }
            return true;
        }

        public static List<AuditData> GetAuditDatas(string ObjectName, string ObjectID)
        {
            List<AuditData> objReturn = new List<AuditData>();
            ;
            AuditData objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.*,u.UserName " +
                         "FROM AuditData s WITH (NOLOCK) " +
                         "INNER JOIN  AspNetUsers u on u.Id = s.CreatedBy" +
                         " where (s.ObjectName = " + Database.HandleQuote(ObjectName) +
                         " AND s.ObjectID=" + Database.HandleQuote(ObjectID) +
                         ") OR (s.ParentObject =" + Database.HandleQuote(ObjectName) +
                         " AND s.ParentKey =" + Database.HandleQuote(ObjectID) + ") Order By CreatedOn DESC";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AuditData(objData.Tables[0].Rows[i]);
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
