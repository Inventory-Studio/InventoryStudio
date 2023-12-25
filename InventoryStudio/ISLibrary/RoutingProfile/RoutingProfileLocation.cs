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
    public class RoutingProfileLocation : BaseClass
    {
        public string RoutingProfileLocationID { get; set; } = string.Empty;
        public bool IsNew { get { return string.IsNullOrEmpty(RoutingProfileLocationID); } }
        public string RoutingProfileID { get; set; } = string.Empty;
        public string LocationID { get; set; } = string.Empty;
        public int Ordering { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

        public RoutingProfileLocation()
        {
        }
        public RoutingProfileLocation(string RoutingProfileLocationID)
        {
            this.RoutingProfileLocationID = RoutingProfileLocationID;
            Load();
        }
        public RoutingProfileLocation(DataRow objRow)
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
                         "FROM RoutingProfileLocation (NOLOCK) " +
                         "WHERE RoutingProfileLocationID=" + Database.HandleQuote(RoutingProfileLocationID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RoutingProfileLocationID=" + RoutingProfileLocationID + " is not found");
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

                if (objColumns.Contains("RoutingProfileLocationID")) RoutingProfileLocationID = Convert.ToString(objRow["RoutingProfileLocationID"]);
                if (objColumns.Contains("RoutingProfileID")) RoutingProfileID = Convert.ToString(objRow["RoutingProfileID"]);
                if (objColumns.Contains("LocationID")) LocationID = Convert.ToString(objRow["LocationID"]);
                if (objColumns.Contains("Ordering")) Ordering = Convert.ToInt32(objRow["Ordering"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(RoutingProfileLocationID)) throw new Exception("Missing RoutingProfileLocationID in the datarow");
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
                if (string.IsNullOrEmpty(RoutingProfileID)) throw new Exception("RoutingProfileID is required");
                if (string.IsNullOrEmpty(LocationID)) throw new Exception("LocationID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, RoutingProfileLocationID already exists");

                dicParam["RoutingProfileID"] = RoutingProfileID;
                dicParam["LocationID"] = LocationID;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = CreatedOn;
                RoutingProfileLocationID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "RoutingProfileLocation"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(RoutingProfileID)) throw new Exception("RoutingProfileID is required");
                if (string.IsNullOrEmpty(LocationID)) throw new Exception("LocationID is required");
                if (IsNew) throw new Exception("Update cannot be performed, RoutingProfileLocationID is missing");

                dicParam["RoutingProfileID"] = RoutingProfileID;
                dicParam["LocationID"] = LocationID;
                dicParam["Ordering"] = Ordering;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["RoutingProfileLocationID"] = RoutingProfileLocationID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "RoutingProfileLocation"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, RoutingProfileLocationID is missing");

                dicDParam["RoutingProfileLocationID"] = RoutingProfileLocationID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "RoutingProfileLocation"), objConn, objTran);
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

        public static RoutingProfileLocation GetRoutingProfileLocation(RoutingProfileLocationFilter Filter)
        {
            List<RoutingProfileLocation> objRoutingProfileLocations = null;
            RoutingProfileLocation objReturn = null;

            try
            {
                objRoutingProfileLocations = GetRoutingProfileLocations(Filter);
                if (objRoutingProfileLocations != null && objRoutingProfileLocations.Count >= 1) objReturn = objRoutingProfileLocations[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRoutingProfileLocations = null;
            }
            return objReturn;
        }

        public static List<RoutingProfileLocation> GetRoutingProfileLocations()
        {
            int intTotalCount = 0;
            return GetRoutingProfileLocations(null, null, null, out intTotalCount);
        }

        public static List<RoutingProfileLocation> GetRoutingProfileLocations(RoutingProfileLocationFilter Filter)
        {
            int intTotalCount = 0;
            return GetRoutingProfileLocations(Filter, null, null, out intTotalCount);
        }

        public static List<RoutingProfileLocation> GetRoutingProfileLocations(RoutingProfileLocationFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRoutingProfileLocations(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<RoutingProfileLocation> GetRoutingProfileLocations(RoutingProfileLocationFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<RoutingProfileLocation> objReturn = null;
            RoutingProfileLocation objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<RoutingProfileLocation>();

                strSQL = "SELECT s.* " +
                         "FROM RoutingProfileLocation (NOLOCK) s ";

                if (Filter != null)
                {
                    if (Filter.RoutingProfileID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RoutingProfileID, "RoutingProfileID");
                    if (Filter.LocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LocationID, "LocationID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RoutingProfileLocationID" : Utility.CustomSorting.GetSortExpression(typeof(RoutingProfileLocation), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new RoutingProfileLocation(objData.Tables[0].Rows[i]);
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
