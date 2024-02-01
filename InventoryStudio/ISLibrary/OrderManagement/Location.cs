using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class Location : BaseClass
    {
        public string LocationID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(LocationID); } }

        public string CompanyID { get; set; }

        public string? ParentLocationID { get; set; }

        public string? LocationNumber { get; set; }

        public string LocationName { get; set; }

        public bool UseBins { get; set; }

        public bool UseLotNumber { get; set; }

        public bool UseCartonNumber { get; set; }

        public bool UseVendorCartonNumber { get; set; }

        public bool UseSerialNumber { get; set; }

        public bool AllowMultiplePackagePerFulfillment { get; set; }

        public bool AllowAutoPick { get; set; }

        public bool AllowAutoPickApproval { get; set; }

        public bool AllowNegativeInventory { get; set; }

        public bool DefaultAddressValidation { get; set; }

        public bool DefaultSignatureRequirement { get; set; }

        public decimal? DefaultSignatureRequirementAmount { get; set; }

        public string? DefaultCountryOfOrigin { get; set; }

        public string? DefaultHSCode { get; set; }

        public bool DefaultLowestShippingRate { get; set; }

        public int? MaximumPickScanRequirement { get; set; }

        public int? MaximumPackScanRequirement { get; set; }

        public string? DisplayWeightMode { get; set; }

        public string? FulfillmentCombineStatus { get; set; }

        public string? DefaultPackageDimensionID { get; set; }

        public bool EnableSimpleMode { get; set; }

        public bool EnableSimpleModePick { get; set; }

        public bool EnableSimpleModePack { get; set; }

        public bool ValidateSource { get; set; }

        public string? AddressID { get; set; }

        public string? VarianceBinID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Location()
        {

        }

        public Location(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public Location(string CompanyID, string LocationID)
        {
            this.CompanyID = CompanyID;
            this.LocationID = LocationID;
            Load();
        }

        public Location(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ParentLocationID")) ParentLocationID = Convert.ToString(objRow["ParentLocationID"]);
                if (objColumns.Contains("LocationNumber")) LocationNumber = Convert.ToString(objRow["LocationNumber"]);
                if (objColumns.Contains("LocationName")) LocationName = Convert.ToString(objRow["LocationName"]);
                if (objColumns.Contains("UseBins")) UseBins = Convert.ToBoolean(objRow["UseBins"]);
                if (objColumns.Contains("UseLotNumber")) UseLotNumber = Convert.ToBoolean(objRow["UseLotNumber"]);
                if (objColumns.Contains("UseCartonNumber")) UseCartonNumber = Convert.ToBoolean(objRow["UseCartonNumber"]);
                if (objColumns.Contains("UseVendorCartonNumber")) UseVendorCartonNumber = Convert.ToBoolean(objRow["UseVendorCartonNumber"]);
                if (objColumns.Contains("UseSerialNumber")) UseSerialNumber = Convert.ToBoolean(objRow["UseSerialNumber"]);
                if (objColumns.Contains("AllowMultiplePackagePerFulfillment")) AllowMultiplePackagePerFulfillment = Convert.ToBoolean(objRow["AllowMultiplePackagePerFulfillment"]);
                if (objColumns.Contains("AllowAutoPick")) AllowAutoPick = Convert.ToBoolean(objRow["AllowAutoPick"]);
                if (objColumns.Contains("AllowAutoPickApproval")) AllowAutoPickApproval = Convert.ToBoolean(objRow["AllowAutoPickApproval"]);
                if (objColumns.Contains("AllowNegativeInventory")) AllowNegativeInventory = Convert.ToBoolean(objRow["AllowNegativeInventory"]);
                if (objColumns.Contains("DefaultAddressValidation")) DefaultAddressValidation = Convert.ToBoolean(objRow["DefaultAddressValidation"]);
                if (objColumns.Contains("DefaultSignatureRequirement")) DefaultSignatureRequirement = Convert.ToBoolean(objRow["DefaultSignatureRequirement"]);
                if (objColumns.Contains("DefaultSignatureRequirementAmount")) DefaultSignatureRequirementAmount = objRow["DefaultSignatureRequirementAmount"] == DBNull.Value ? null : Convert.ToDecimal(objRow["DefaultSignatureRequirementAmount"]);
                if (objColumns.Contains("DefaultCountryOfOrigin")) DefaultCountryOfOrigin = Convert.ToString(objRow["DefaultCountryOfOrigin"]);
                if (objColumns.Contains("DefaultHSCode")) DefaultHSCode = Convert.ToString(objRow["DefaultHSCode"]);
                if (objColumns.Contains("DefaultLowestShippingRate")) DefaultLowestShippingRate = Convert.ToBoolean(objRow["DefaultLowestShippingRate"]);
                if (objColumns.Contains("MaximumPickScanRequirement")) MaximumPickScanRequirement = objRow["MaximumPickScanRequirement"] == DBNull.Value ? null : Convert.ToInt32(objRow["MaximumPickScanRequirement"]);
                if (objColumns.Contains("MaximumPackScanRequirement")) MaximumPackScanRequirement = objRow["MaximumPackScanRequirement"] == DBNull.Value ? null : Convert.ToInt32(objRow["MaximumPackScanRequirement"]);
                if (objColumns.Contains("DisplayWeightMode")) DisplayWeightMode = Convert.ToString(objRow["DisplayWeightMode"]);
                if (objColumns.Contains("FulfillmentCombineStatus")) FulfillmentCombineStatus = Convert.ToString(objRow["FulfillmentCombineStatus"]);
                if (objColumns.Contains("DefaultPackageDimensionID")) DefaultPackageDimensionID = objRow["DefaultPackageDimensionID"] == DBNull.Value ? null : Convert.ToString(objRow["DefaultPackageDimensionID"]);
                if (objColumns.Contains("EnableSimpleMode")) EnableSimpleMode = Convert.ToBoolean(objRow["EnableSimpleMode"]);
                if (objColumns.Contains("EnableSimpleModePick")) EnableSimpleModePick = Convert.ToBoolean(objRow["EnableSimpleModePick"]);
                if (objColumns.Contains("EnableSimpleModePack")) EnableSimpleModePack = Convert.ToBoolean(objRow["EnableSimpleModePack"]);
                if (objColumns.Contains("ValidateSource")) ValidateSource = Convert.ToBoolean(objRow["ValidateSource"]);
                if (objColumns.Contains("AddressID")) AddressID = Convert.ToString(objRow["AddressID"]);
                if (objColumns.Contains("VarianceBinID")) VarianceBinID = Convert.ToString(objRow["VarianceBinID"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn")) UpdatedOn = objRow["UpdatedOn"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
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


        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.* " +
                         "FROM Location l (NOLOCK) " +
                         "WHERE l.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND l.LocationID = " + Database.HandleQuote(LocationID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Location is not found");
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
            return base.Create();
        }


        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();
            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(LocationName)) throw new Exception("LocationName is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentLocationID"] = ParentLocationID;
                dicParam["LocationNumber"] = LocationNumber;
                dicParam["LocationName"] = LocationName;
                dicParam["UseBins"] = UseBins;
                dicParam["UseLotNumber"] = UseLotNumber;
                dicParam["UseCartonNumber"] = UseCartonNumber;
                dicParam["UseVendorCartonNumber"] = UseVendorCartonNumber;
                dicParam["UseSerialNumber"] = UseSerialNumber;
                dicParam["AllowMultiplePackagePerFulfillment"] = AllowMultiplePackagePerFulfillment;
                dicParam["AllowAutoPick"] = AllowAutoPick;
                dicParam["AllowAutoPickApproval"] = AllowAutoPickApproval;
                dicParam["AllowNegativeInventory"] = AllowNegativeInventory;
                dicParam["DefaultAddressValidation"] = DefaultAddressValidation;
                dicParam["DefaultSignatureRequirement"] = DefaultSignatureRequirement;
                dicParam["DefaultSignatureRequirementAmount"] = DefaultSignatureRequirementAmount;
                dicParam["DefaultCountryOfOrigin"] = DefaultCountryOfOrigin;
                dicParam["DefaultHSCode"] = DefaultHSCode;
                dicParam["DefaultLowestShippingRate"] = DefaultLowestShippingRate;
                dicParam["MaximumPickScanRequirement"] = MaximumPickScanRequirement;
                dicParam["MaximumPackScanRequirement"] = MaximumPackScanRequirement;
                dicParam["DisplayWeightMode"] = DisplayWeightMode;
                dicParam["FulfillmentCombineStatus"] = FulfillmentCombineStatus;
                dicParam["DefaultPackageDimensionID"] = DefaultPackageDimensionID;
                dicParam["EnableSimpleMode"] = EnableSimpleMode;
                dicParam["EnableSimpleModePick"] = EnableSimpleModePick;
                dicParam["EnableSimpleModePack"] = EnableSimpleModePack;
                dicParam["ValidateSource"] = ValidateSource;
                dicParam["AddressID"] = AddressID;
                dicParam["VarianceBinID"] = VarianceBinID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;

                LocationID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Location"), objConn, objTran).ToString();

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
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(LocationName)) throw new Exception("LocationName is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["ParentLocationID"] = ParentLocationID;
                dicParam["LocationNumber"] = LocationNumber;
                dicParam["LocationName"] = LocationName;
                dicParam["UseBins"] = UseBins;
                dicParam["UseLotNumber"] = UseLotNumber;
                dicParam["UseCartonNumber"] = UseCartonNumber;
                dicParam["UseVendorCartonNumber"] = UseVendorCartonNumber;
                dicParam["UseSerialNumber"] = UseSerialNumber;
                dicParam["AllowMultiplePackagePerFulfillment"] = AllowMultiplePackagePerFulfillment;
                dicParam["AllowAutoPick"] = AllowAutoPick;
                dicParam["AllowAutoPickApproval"] = AllowAutoPickApproval;
                dicParam["AllowNegativeInventory"] = AllowNegativeInventory;
                dicParam["DefaultAddressValidation"] = DefaultAddressValidation;
                dicParam["DefaultSignatureRequirement"] = DefaultSignatureRequirement;
                dicParam["DefaultSignatureRequirementAmount"] = DefaultSignatureRequirementAmount;
                dicParam["DefaultCountryOfOrigin"] = DefaultCountryOfOrigin;
                dicParam["DefaultHSCode"] = DefaultHSCode;
                dicParam["DefaultLowestShippingRate"] = DefaultLowestShippingRate;
                dicParam["MaximumPickScanRequirement"] = MaximumPickScanRequirement;
                dicParam["MaximumPackScanRequirement"] = MaximumPackScanRequirement;
                dicParam["DisplayWeightMode"] = DisplayWeightMode;
                dicParam["FulfillmentCombineStatus"] = FulfillmentCombineStatus;
                dicParam["DefaultPackageDimensionID"] = DefaultPackageDimensionID;
                dicParam["EnableSimpleMode"] = EnableSimpleMode;
                dicParam["EnableSimpleModePick"] = EnableSimpleModePick;
                dicParam["EnableSimpleModePack"] = EnableSimpleModePack;
                dicParam["ValidateSource"] = ValidateSource;
                dicParam["AddressID"] = AddressID;
                dicParam["VarianceBinID"] = VarianceBinID;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;

                dicWParam["LocationID"] = LocationID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Location"), objConn, objTran);
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
            base.Update();
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
            if (!IsLoaded) Load();

            base.Delete();

            Hashtable dicDParam = new Hashtable();
            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, LocationID is missing");
                dicDParam["LocationID"] = LocationID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Location"), objConn, objTran);
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
            strSQL = "SELECT TOP 1 l.* " +
                     "FROM Location (NOLOCK) l " +
                     "WHERE l.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND l.LocationID=" + Database.HandleQuote(LocationID);
            return Database.HasRows(strSQL);
        }

        public static Location GetLocation(string CompanyID, LocationFilter Filter)
        {
            List<Location> objItems = null;
            Location objReturn = null;

            try
            {
                objItems = GetLocations(CompanyID, Filter);
                if (objItems != null && objItems.Count >= 1) objReturn = objItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItems = null;
            }
            return objReturn;
        }

        public static List<Location> GetLocations(string CompanyID)
        {
            int intTotalCount = 0;
            return GetLocations(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<Location> GetLocations(string CompanyID, LocationFilter Filter)
        {
            int intTotalCount = 0;
            return GetLocations(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<Location> GetLocations(string CompanyID, LocationFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetLocations(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Location> GetLocations(string CompanyID, LocationFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Location> objReturn = null;
            Location objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Location>();

                strSQL = "SELECT p.* " +
                         "FROM Location (NOLOCK) l " +
                         "WHERE l.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "p.LocationID");
                    if (Filter.ParentLocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentLocationID, "p.ParentLocationID");

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PackageDimensionID" : Utility.CustomSorting.GetSortExpression(typeof(Location), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Location(objData.Tables[0].Rows[i]);
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
                objNew = null;
            }
            return objReturn;
        }
    }
}
