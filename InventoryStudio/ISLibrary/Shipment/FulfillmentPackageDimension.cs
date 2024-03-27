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
    public class FulfillmentPackageDimension : BaseClass
    {
        public string FulfillmentPackageDimensionID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(FulfillmentPackageDimensionID); }
        }     
        public string CompanyID { get; set; }
        public string Label { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal WeightUnit { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }



        public FulfillmentPackageDimension()
        {
        }

        public FulfillmentPackageDimension( string CompanyID, string FulfillmentPackageDimensionID)
        {
            this.CompanyID = CompanyID;
            this.FulfillmentPackageDimensionID = FulfillmentPackageDimensionID;
            this.Load();
        }

        public FulfillmentPackageDimension(DataRow objRow)
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
                         "FROM FulfillmentPackageDimension (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                        " AND FulfillmentPackageDimensionID=" + Database.HandleQuote(FulfillmentPackageDimensionID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentPackageDimensionID=" + FulfillmentPackageDimensionID + " is not found");
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

                if (objColumns.Contains("FulfillmentPackageDimensionID")) FulfillmentPackageDimensionID = Convert.ToString(objRow["FulfillmentPackageDimensionID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("Label")) Label = Convert.ToString(objRow["Label"]);
                if (objColumns.Contains("Length")) Length = Convert.ToDecimal(objRow["Length"]);
                if (objColumns.Contains("Width")) Width = Convert.ToDecimal(objRow["Width"]);
                if (objColumns.Contains("Height")) Height = Convert.ToDecimal(objRow["Height"]);
                if (objColumns.Contains("Weight")) Weight = Convert.ToDecimal(objRow["Weight"]);
                if (objColumns.Contains("WeightUnit")) WeightUnit = Convert.ToDecimal(objRow["WeightUnit"]);


                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
           

                if (string.IsNullOrEmpty(FulfillmentPackageDimensionID)) throw new Exception("Missing FulfillmentPackageDimensionID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, FulfillmentPackageLineID already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["Label"] = Label;
                dicParam["Length"] = Length;
                dicParam["Width"] = Width;
                dicParam["Height"] = Height;
                dicParam["Weight"] = Weight;
                dicParam["WeightUnit"] = WeightUnit;

                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                FulfillmentPackageDimensionID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "FulfillmentPackageDimension"), objConn, objTran)
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


        //public static FulfillmentLine GetGetFulfillmentLine(string CompanyID, FulfillmentLineFilter Filter)
        //{
        //    List<FulfillmentLine> objAdjustments = null;
        //    FulfillmentLine objReturn = null;

        //    try
        //    {
        //        objAdjustments = GetFulfillmentLines(CompanyID, Filter);
        //        if (objAdjustments != null && objAdjustments.Count >= 1) objReturn = objAdjustments[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objAdjustments = null;
        //    }

        //    return objReturn;
        //}

        //public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID)
        //{
        //    int intTotalCount = 0;
        //    return GetFulfillmentLines(CompanyID, null, null, null, out intTotalCount);
        //}

        //public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID, FulfillmentLineFilter Filter)
        //{
        //    int intTotalCount = 0;
        //    return GetFulfillmentLines(CompanyID, Filter, null, null, out intTotalCount);
        //}

        //public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID, FulfillmentLineFilter Filter, int? PageSize,
        //    int? PageNumber, out int TotalRecord)
        //{
        //    return GetFulfillmentLines(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        //}

        //public static List<FulfillmentLine> GetFulfillmentLines(string CompanyID, FulfillmentLineFilter Filter,
        //    string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        //{
        //    List<FulfillmentLine> objReturn = null;
        //    FulfillmentLine objNew = null;
        //    DataSet objData = null;
        //    string strSQL = string.Empty;

        //    try
        //    {
        //        TotalRecord = 0;

        //        objReturn = new List<FulfillmentLine>();

        //        strSQL = "SELECT * " +
        //                 "FROM FulfillmentLine (NOLOCK) " +
        //                 "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
        //        if (Filter != null)
        //        {
        //            if (Filter.FulfillmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FulfillmentID, "FulfillmentID");
        //        }

        //        if (PageSize != null && PageNumber != null)
        //            strSQL = Database.GetPagingSQL(strSQL,
        //                string.IsNullOrEmpty(SortExpression)
        //                    ? "FulfillmentLineID"
        //                    : Utility.CustomSorting.GetSortExpression(typeof(FulfillmentLine), SortExpression),
        //                string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
        //        objData = Database.GetDataSet(strSQL);

        //        if (objData != null && objData.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
        //            {
        //                objNew = new FulfillmentLine(objData.Tables[0].Rows[i]);
        //                objNew.IsLoaded = true;
        //                objReturn.Add(objNew);
        //            }
        //        }

        //        TotalRecord = objReturn.Count();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objData = null;
        //    }

        //    return objReturn;
        //}


    }
}