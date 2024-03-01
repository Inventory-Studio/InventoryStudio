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
    public class Adjustment : BaseClass
    {
        public string AdjustmentID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(AdjustmentID); }
        }        
        public string CompanyID { get; set; }
        public DateTime? TranDate { get; set; }
        public string LocationID { get; set; }
        //public string LocationName { get; set; }
        public string Memo { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        private List<AdjustmentLine> mAdjustmentLine = null;
        public List<AdjustmentLine> AdjustmentLines
        {
            get
            {
                AdjustmentLineFilter objFilter = null;

                try
                {
                    if (mAdjustmentLine == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(AdjustmentID))
                    {
                        objFilter = new AdjustmentLineFilter();
                        objFilter.AdjustmentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.AdjustmentID.SearchString = AdjustmentID;
                        mAdjustmentLine = AdjustmentLine.GetAdjustmentLines(CompanyID, objFilter);
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
                return mAdjustmentLine;
            }
            set
            {
                mAdjustmentLine = value;
            }
        }
    
        public Adjustment()
        {
        }

        public Adjustment( /*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.AdjustmentID = Id;
            this.Load();
        }

        public Adjustment(DataRow objRow)
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
                         "FROM Adjustment (NOLOCK) " +
                         "WHERE AdjustmentID=" + Database.HandleQuote(AdjustmentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AdjustmentID=" + AdjustmentID + " is not found");
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

                if (objColumns.Contains("AdjustmentID")) AdjustmentID = Convert.ToString(objRow["AdjustmentID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                //if (objColumns.Contains("LocationName")) LocationName = Convert.ToString(objRow["LocationName"]);
                if (objColumns.Contains("Memo")) Memo = Convert.ToString(objRow["Memo"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("TranDate")) TranDate = Convert.ToDateTime(objRow["TranDate"]);

                if (string.IsNullOrEmpty(AdjustmentID)) throw new Exception("Missing AdjustmentID in the datarow");
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
                dicParam["TranDate"] = DateTime.UtcNow; 
                dicParam["LocationID"] = LocationID;
                dicParam["Memo"] = Memo;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                AdjustmentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Adjustment"), objConn, objTran)
                    .ToString();

                if (AdjustmentLines != null)
                {
                    foreach (AdjustmentLine objAdjustmentLine in AdjustmentLines)
                    {
                        if (objAdjustmentLine.IsNew)
                        {
                            objAdjustmentLine.CompanyID = CompanyID;
                            objAdjustmentLine.AdjustmentID = AdjustmentID;
                            objAdjustmentLine.CreatedBy = CreatedBy;
                            objAdjustmentLine.ParentKey = AdjustmentID;
                            objAdjustmentLine.ParentObject = "Adjustment";
                            objAdjustmentLine.Create(objConn, objTran);
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


        public static Adjustment GetAdjustment(string CompanyID, AdjustmentFilter Filter)
        {
            List<Adjustment> objAdjustments = null;
            Adjustment objReturn = null;

            try
            {
                objAdjustments = GetAdjustments(CompanyID, Filter);
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

        public static List<Adjustment> GetAdjustments(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAdjustments(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Adjustment> GetAdjustments(string CompanyID, AdjustmentFilter Filter)
        {
            int intTotalCount = 0;
            return GetAdjustments(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Adjustment> GetAdjustments(string CompanyID, AdjustmentFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetAdjustments(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Adjustment> GetAdjustments(string CompanyID, AdjustmentFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Adjustment> objReturn = null;
            Adjustment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Adjustment>();

                strSQL = "SELECT * " +
                         "FROM Adjustment (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "LocationID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "AdjustmentID"
                            : Utility.CustomSorting.GetSortExpression(typeof(Adjustment), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Adjustment(objData.Tables[0].Rows[i]);
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