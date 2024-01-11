using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Net.NetworkInformation;
using CLRFramework;
using Microsoft.Data.SqlClient;

namespace ISLibrary
{
    public class RoutingProfile : BaseClass
    {
        public string RoutingProfileID { get; set; } = string.Empty;
        public bool IsNew { get { return string.IsNullOrEmpty(RoutingProfileID); } }
        public string Name { get; set; } = string.Empty;
        public Routing.enumFulfillmentMethod FulfillmentMethod { get; set; }
        public Routing.enumFulfillmentStrategy FulfillmentStrategy { get; set; }
        public Routing.enumAllocationStrategy AllocationStrategy { get; set; }
        public bool ApprovalRequired { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        private List<RoutingProfileLocation>? mRoutingProfileLocations = null;
        public List<RoutingProfileLocation>? RoutingProfileLocations
        {
            get
            {
                if (mRoutingProfileLocations == null && !string.IsNullOrEmpty(RoutingProfileID))
                {
                    RoutingProfileLocationFilter? objFilter = null;

                    try
                    {
                        objFilter = new RoutingProfileLocationFilter();
                        objFilter.RoutingProfileID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.RoutingProfileID.SearchString = RoutingProfileID;
                        mRoutingProfileLocations = RoutingProfileLocation.GetRoutingProfileLocations(objFilter).OrderBy(s => s.Ordering).ToList();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mRoutingProfileLocations;
            }
        }

        public RoutingProfile()
        {
        }

        public RoutingProfile(string RoutingProfileID)
        {
            this.RoutingProfileID = RoutingProfileID;
            Load();
        }

        public RoutingProfile(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM RoutingProfile (NOLOCK) " +
                         "WHERE RoutingProfileID=" + Database.HandleQuote(RoutingProfileID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RoutingProfileID=" + RoutingProfileID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

                if (objColumns.Contains("RoutingProfileID")) RoutingProfileID = Convert.ToString(objRow["RoutingProfileID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("FulfillmentMethod")) FulfillmentMethod = (Routing.enumFulfillmentMethod)Enum.Parse(typeof(Routing.enumFulfillmentMethod), Convert.ToString(objRow["FulfillmentMethod"]), true);
                if (objColumns.Contains("FulfillmentStrategy")) FulfillmentStrategy = (Routing.enumFulfillmentStrategy)Enum.Parse(typeof(Routing.enumFulfillmentStrategy), Convert.ToString(objRow["FulfillmentStrategy"]), true);
                if (objColumns.Contains("AllocationStrategy")) AllocationStrategy = (Routing.enumAllocationStrategy)Enum.Parse(typeof(Routing.enumAllocationStrategy), Convert.ToString(objRow["AllocationStrategy"]), true);
                if (objColumns.Contains("ApprovalRequired")) ApprovalRequired = Convert.ToBoolean(objRow["ApprovalRequired"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(RoutingProfileID)) throw new Exception("Missing RoutingProfileID in the datarow");
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
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (!IsNew) throw new Exception("Create cannot be performed, RoutingProfileID already exists");

                dicParam["Name"] = Name;
                dicParam["FulfillmentMethod"] = Utility.EnumHelper.GetDescription(FulfillmentMethod);
                dicParam["FulfillmentStrategy"] = Utility.EnumHelper.GetDescription(FulfillmentStrategy);
                dicParam["AllocationStrategy"] = Utility.EnumHelper.GetDescription(AllocationStrategy);
                dicParam["ApprovalRequired"] = ApprovalRequired;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;
                RoutingProfileID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "RoutingProfile"), objConn, objTran).ToString();

                //foreach (RoutingProfileLine objRoutingProfileLine in RoutingProfileLines)
                //{
                //    objRoutingProfileLine.RoutingProfileID = RoutingProfileID;
                //    objRoutingProfileLine.Create(objConn, objTran);
                //}

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
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (IsNew) throw new Exception("Update cannot be performed, RoutingProfileID is missing");

                dicParam["Name"] = Name;
                dicParam["FulfillmentMethod"] = Utility.EnumHelper.GetDescription(FulfillmentMethod);
                dicParam["FulfillmentStrategy"] = Utility.EnumHelper.GetDescription(FulfillmentStrategy);
                dicParam["AllocationStrategy"] = Utility.EnumHelper.GetDescription(AllocationStrategy);
                dicParam["ApprovalRequired"] = ApprovalRequired;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["RoutingProfileID"] = RoutingProfileID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "RoutingProfile"), objConn, objTran);

                //foreach (RoutingProfileLine objRoutingProfileLine in RoutingProfileLines)
                //{
                //    objRoutingProfileLine.Update(objConn, objTran);
                //}

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
                if (IsNew) throw new Exception("Delete cannot be performed, RoutingProfileID is missing");

                dicDParam["RoutingProfileID"] = RoutingProfileID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "RoutingProfile"), objConn, objTran);
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

        public static RoutingProfile GetRoutingProfile(RoutingProfileFilter Filter)
        {
            List<RoutingProfile> objRoutingProfiles = null;
            RoutingProfile objReturn = null;

            try
            {
                objRoutingProfiles = GetRoutingProfiles(Filter);
                if (objRoutingProfiles != null && objRoutingProfiles.Count >= 1) objReturn = objRoutingProfiles[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRoutingProfiles = null;
            }
            return objReturn;
        }

        public static List<RoutingProfile> GetRoutingProfiles()
        {
            int intTotalCount = 0;
            return GetRoutingProfiles(null, null, null, out intTotalCount);
        }

        public static List<RoutingProfile> GetRoutingProfiles(RoutingProfileFilter Filter)
        {
            int intTotalCount = 0;
            return GetRoutingProfiles(Filter, null, null, out intTotalCount);
        }

        public static List<RoutingProfile> GetRoutingProfiles(RoutingProfileFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRoutingProfiles(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<RoutingProfile> GetRoutingProfiles(RoutingProfileFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<RoutingProfile> objReturn = null;
            RoutingProfile objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<RoutingProfile>();

                strSQL = "SELECT s.* " +
                         "FROM RoutingProfile (NOLOCK) s ";

                //if (Filter.DelaySuspectedFraudOrder != null && Filter.DelaySuspectedFraudOrder.Value)
                //{
                //    strSQL += @"LEFT OUTER JOIN AddressTrans ba ON s.BillingAddressTransID=ba.AddressTransID
                //                LEFT OUTER JOIN AddressTrans sa ON s.DeliveryAddressTransID=sa.AddressTransID ";
                //}

                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.FulfillmentMethods != null && Filter.FulfillmentMethods.Count > 0) strSQL += "AND FulfillmentMethod IN (" + String.Join(",", Filter.FulfillmentMethods.Select(m => Database.HandleQuote(Utility.EnumHelper.GetDescription(m))).ToArray()) + ") ";
                    if (Filter.FulfillmentStrategies != null && Filter.FulfillmentStrategies.Count > 0) strSQL += "AND FulfillmentStrategy IN (" + String.Join(",", Filter.FulfillmentStrategies.Select(m => Database.HandleQuote(Utility.EnumHelper.GetDescription(m))).ToArray()) + ") ";
                    if (Filter.AllocationStrategies != null && Filter.AllocationStrategies.Count > 0) strSQL += "AND AllocationStrategy IN (" + String.Join(",", Filter.AllocationStrategies.Select(m => Database.HandleQuote(Utility.EnumHelper.GetDescription(m))).ToArray()) + ") ";
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RoutingProfileID" : Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new RoutingProfile(objData.Tables[0].Rows[i]);
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
            }
            return objReturn;
        }
    }
}
