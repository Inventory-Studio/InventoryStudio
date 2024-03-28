using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;
using ISLibrary.OrderManagement;
using System.Data.Common;
using System.Transactions;
using System.ComponentModel;

namespace ISLibrary
{
    public class FulfillmentPackage : BaseClass
    {
        public string FulfillmentPackageID { get; set; }
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(FulfillmentPackageID); }
        }


        public string CompanyID { get; set; }
        public string FulfillmentID { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string WeightUnit { get; set; }
        public string ShippingPackage { get; set; }
        public string Template { get; set; }
        public string ShippingExternalID { get; set; }
        public string ShippingCarrierID { get; set; }
        public string ShippingServiceID { get; set; }
        public string ShippingPackageID { get; set; }
        public string PackageDimensionID { get; set; }
        public decimal ShippingRate { get; set; }
        public decimal PackageCount { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public bool IsSignatureRequired { get; set; }
        public string ShippingLabelPath { get; set; }
        public string CommercialInvoicePath { get; set; }
        public string CertificateOfOriginPath { get; set; }
        public DateTime FirstScannedOn { get; set; }
        public bool IsPDF { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<FulfillmentPackageLine> mFulfillmentPackageLines = null;
        public List<FulfillmentPackageLine> FulfillmentPackageLines
        {
            get
            {
                FulfillmentPackageLineFilter objFilter = null;

                try
                {
                    if (mFulfillmentPackageLines == null && !string.IsNullOrEmpty(CompanyID) && !string.IsNullOrEmpty(FulfillmentPackageID))
                    {
                        objFilter = new FulfillmentPackageLineFilter();
                        objFilter.FulfillmentPackageID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.FulfillmentPackageID.SearchString = FulfillmentPackageID;
                        mFulfillmentPackageLines = FulfillmentPackageLine.GetFulfillmentPackageLines(CompanyID, objFilter);
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
                return mFulfillmentPackageLines;
            }
            set
            {
                mFulfillmentPackageLines = value;
            }
        }




        public FulfillmentPackage()
        {
        }

        public FulfillmentPackage( string CompanyID, string FulfillmentPackageID)
        {
            this.CompanyID = CompanyID;
            this.FulfillmentPackageID = FulfillmentPackageID;
            this.Load();
        }

        public FulfillmentPackage(DataRow objRow)
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
                         "FROM FulfillmentPackage (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID) +
                        " AND FulfillmentPackageID=" + Database.HandleQuote(FulfillmentPackageID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentPackageID=" + FulfillmentPackageID + " is not found");
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

                if (objColumns.Contains("FulfillmentPackageID")) FulfillmentPackageID = Convert.ToString(objRow["FulfillmentPackageID"]);
                if (objColumns.Contains("FulfillmentID")) FulfillmentID = Convert.ToString(objRow["FulfillmentID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("Length")) Length = Convert.ToDecimal(objRow["Length"]);
                if (objColumns.Contains("Width")) Width = Convert.ToDecimal(objRow["Width"]);
                if (objColumns.Contains("Height")) Height = Convert.ToDecimal(objRow["Height"]);
                if (objColumns.Contains("Weight")) Weight = Convert.ToDecimal(objRow["Weight"]);
                if (objColumns.Contains("WeightUnit")) WeightUnit = Convert.ToString(objRow["WeightUnit"]);
                if (objColumns.Contains("ShippingPackage")) ShippingPackage = Convert.ToString(objRow["ShippingPackage"]);
                if (objColumns.Contains("Template")) Template = Convert.ToString(objRow["Template"]);
                if (objColumns.Contains("ShippingExternalID")) ShippingExternalID = Convert.ToString(objRow["ShippingExternalID"]);
                if (objColumns.Contains("ShippingCarrierID")) ShippingCarrierID = Convert.ToString(objRow["ShippingCarrierID"]);
                if (objColumns.Contains("ShippingServiceID")) ShippingServiceID = Convert.ToString(objRow["ShippingServiceID"]);
                if (objColumns.Contains("ShippingPackageID")) ShippingPackageID = Convert.ToString(objRow["ShippingPackageID"]);
                if (objColumns.Contains("PackageDimensionID")) PackageDimensionID = Convert.ToString(objRow["PackageDimensionID"]);
                if (objColumns.Contains("ShippingRate")) ShippingRate = Convert.ToDecimal(objRow["ShippingRate"]);
                if (objColumns.Contains("PackageCount")) PackageCount = Convert.ToDecimal(objRow["PackageCount"]);
                if (objColumns.Contains("TrackingNumber")) TrackingNumber = Convert.ToString(objRow["TrackingNumber"]);
                if (objColumns.Contains("Carrier")) Carrier = Convert.ToString(objRow["Carrier"]);
                if (objColumns.Contains("IsSignatureRequired")) IsSignatureRequired = Convert.ToBoolean(objRow["IsSignatureRequired"]);
                if (objColumns.Contains("ShippingLabelPath")) ShippingLabelPath = Convert.ToString(objRow["ShippingLabelPath"]);
                if (objColumns.Contains("CommercialInvoicePath")) CommercialInvoicePath = Convert.ToString(objRow["CommercialInvoicePath"]);
                if (objColumns.Contains("CertificateOfOriginPath")) CertificateOfOriginPath = Convert.ToString(objRow["CertificateOfOriginPath"]);
                if (objColumns.Contains("FirstScannedOn") && objRow["FirstScannedOn"] != DBNull.Value) FirstScannedOn = Convert.ToDateTime(objRow["FirstScannedOn"]);
                if (objColumns.Contains("IsPDF")) IsPDF = Convert.ToBoolean(objRow["IsPDF"]);


                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value)
                    UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
           

                if (string.IsNullOrEmpty(FulfillmentPackageID)) throw new Exception("Missing FulfillmentPackageID in the datarow");
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
                dicParam["FulfillmentID"] = FulfillmentID;
                dicParam["Length"] = Length;
                dicParam["Width"] = Width;
                dicParam["Height"] = Height;
                dicParam["Weight"] = Weight;
                dicParam["WeightUnit"] = WeightUnit;
                dicParam["ShippingPackage"] = ShippingPackage;
                dicParam["Template"] = Template;
                dicParam["ShippingExternalID"] = ShippingExternalID;
                dicParam["ShippingCarrierID"] = ShippingCarrierID;
                dicParam["ShippingServiceID"] = ShippingServiceID;
                dicParam["ShippingPackageID"] = ShippingPackageID;
                dicParam["PackageDimensionID"] = PackageDimensionID;
                dicParam["ShippingRate"] = ShippingRate;
                dicParam["PackageCount"] = PackageCount;
                dicParam["TrackingNumber"] = TrackingNumber;
                dicParam["Carrier"] = Carrier;
                dicParam["IsSignatureRequired"] = IsSignatureRequired;
                dicParam["ShippingLabelPath"] = ShippingLabelPath;
                dicParam["CommercialInvoicePath"] = CommercialInvoicePath;
                dicParam["CertificateOfOriginPath"] = CertificateOfOriginPath;
                if (FirstScannedOn == DateTime.MinValue)
                {
                    dicParam["FirstScannedOn"] = DBNull.Value;
                }
                else
                {
                    dicParam["FirstScannedOn"] = FirstScannedOn;
                }
                dicParam["IsPDF"] = IsPDF;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;


                FulfillmentPackageID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "FulfillmentPackage"), objConn, objTran)
                    .ToString();
                             
             
                if(FulfillmentPackageLines != null)
                {
                    foreach (FulfillmentPackageLine objFulfillmentPackageLine in FulfillmentPackageLines)
                    {
                        if (objFulfillmentPackageLine.IsNew)
                        {
                            objFulfillmentPackageLine.CompanyID = CompanyID;
                            objFulfillmentPackageLine.FulfillmentPackageID = FulfillmentPackageID;
                            objFulfillmentPackageLine.CreatedBy = CreatedBy;
                            objFulfillmentPackageLine.ParentKey = FulfillmentID;
                            objFulfillmentPackageLine.ParentObject = "Fulfillment";
                            objFulfillmentPackageLine.Create(objConn, objTran);
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
            ItemParent objItemParent = null;
            try
            {
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ID is missing");



                dicParam["CompanyID"] = CompanyID;
                dicParam["FulfillmentID"] = FulfillmentID;
                dicParam["Length"] = Length;
                dicParam["Width"] = Width;
                dicParam["Height"] = Height;
                dicParam["Weight"] = Weight;
                dicParam["WeightUnit"] = WeightUnit;
                dicParam["ShippingPackage"] = ShippingPackage;
                dicParam["Template"] = Template;
                dicParam["ShippingExternalID"] = ShippingExternalID;
                dicParam["ShippingCarrierID"] = ShippingCarrierID;
                dicParam["ShippingServiceID"] = ShippingServiceID;
                dicParam["ShippingPackageID"] = ShippingPackageID;
                dicParam["PackageDimensionID"] = PackageDimensionID;
                dicParam["ShippingRate"] = ShippingRate;
                dicParam["PackageCount"] = PackageCount;
                dicParam["TrackingNumber"] = TrackingNumber;
                dicParam["Carrier"] = Carrier;
                dicParam["IsSignatureRequired"] = IsSignatureRequired;
                dicParam["ShippingLabelPath"] = ShippingLabelPath;
                dicParam["CommercialInvoicePath"] = CommercialInvoicePath;
                dicParam["CertificateOfOriginPath"] = CertificateOfOriginPath;
                dicParam["FirstScannedOn"] = FirstScannedOn;
                dicParam["IsPDF"] = IsPDF;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicParam["UpdatedBy"] = UpdatedBy;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "FulfillmentPackage"), objConn, objTran);

              

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

        public static FulfillmentPackage GetFulfillmentPackage(string CompanyID, FulfillmentPackageFilter Filter)
        {
            List<FulfillmentPackage> objAdjustments = null;
            FulfillmentPackage objReturn = null;

            try
            {
                objAdjustments = GetFulfillmentPackages(CompanyID, Filter);
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

        public static List<FulfillmentPackage> GetFulfillmentPackages(string CompanyID)
        {
            int intTotalCount = 0;
            return GetFulfillmentPackages(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<FulfillmentPackage> GetFulfillmentPackages(string CompanyID, FulfillmentPackageFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillmentPackages(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<FulfillmentPackage> GetFulfillmentPackages(string CompanyID, FulfillmentPackageFilter Filter, int? PageSize,
            int? PageNumber, out int TotalRecord)
        {
            return GetFulfillmentPackages(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<FulfillmentPackage> GetFulfillmentPackages(string CompanyID, FulfillmentPackageFilter Filter,
            string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<FulfillmentPackage> objReturn = null;
            FulfillmentPackage objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<FulfillmentPackage>();

                strSQL = "SELECT * " +
                         "FROM FulfillmentPackage (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.FulfillmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FulfillmentID, "FulfillmentID");
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL,
                        string.IsNullOrEmpty(SortExpression)
                            ? "FulfillmentPackageID"
                            : Utility.CustomSorting.GetSortExpression(typeof(FulfillmentPackage), SortExpression),
                        string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new FulfillmentPackage(objData.Tables[0].Rows[i]);
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