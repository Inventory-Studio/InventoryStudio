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
    public class PackageDimension : BaseClass
    {
        public string PackageDimensionID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(PackageDimensionID); } }

        public string? CompanyID { get; set; }

        public string? Name { get; set; }

        public decimal? Length { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public string? WeightUnit { get; set; }

        public decimal? Cost { get; set; }

        public string? ShippingPackage { get; set; }

        public string? Template { get; set; }

        public PackageDimension() { }

        public PackageDimension(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public PackageDimension(string CompanyID, string PackageDimensionID)
        {
            this.CompanyID = CompanyID;
            this.PackageDimensionID = PackageDimensionID;
            Load();
        }

        public PackageDimension(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("PackageDimensionID")) PackageDimensionID = Convert.ToString(objRow["PackageDimensionID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("Length")) Length = objRow["Length"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(objRow["Length"]);
                if (objColumns.Contains("Height")) Height = objRow["Height"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(objRow["Height"]);
                if (objColumns.Contains("Weight")) Weight = objRow["Weight"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(objRow["Weight"]);
                if (objColumns.Contains("WeightUnit")) WeightUnit = Convert.ToString(objRow["WeightUnit"]);
                if (objColumns.Contains("Cost")) Cost = objRow["Cost"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(objRow["Cost"]);
                if (objColumns.Contains("ShippingPackage")) ShippingPackage = Convert.ToString(objRow["ShippingPackage"]);
                if (objColumns.Contains("Template")) Template = Convert.ToString(objRow["Template"]);
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
                strSQL = "SELECT p.* " +
                         "FROM PackageDimension p (NOLOCK) " +
                         "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND p.PackageDimensionID = " + Database.HandleQuote(PackageDimensionID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PackageDimension is not found");
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
                if (string.IsNullOrEmpty(PackageDimensionID)) throw new Exception("PackageDimensionID is required");
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (!IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["Name"] = Name;
                dicParam["Length"] = Length;
                dicParam["Height"] = Height;
                dicParam["Weight"] = Weight;
                dicParam["WeightUnit"] = WeightUnit;
                dicParam["Cost"] = Cost;
                dicParam["ShippingPackage"] = ShippingPackage;
                dicParam["Template"] = Template;

                PackageDimensionID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PackageDimension"), objConn, objTran).ToString();


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
                if (string.IsNullOrEmpty(PackageDimensionID)) throw new Exception("PackageDimensionID is required");
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CompanyID"] = CompanyID;
                dicParam["Name"] = Name;
                dicParam["Length"] = Length;
                dicParam["Height"] = Height;
                dicParam["Weight"] = Weight;
                dicParam["WeightUnit"] = WeightUnit;
                dicParam["Cost"] = Cost;
                dicParam["ShippingPackage"] = ShippingPackage;
                dicParam["Template"] = Template;


                dicWParam["PackageDimensionID"] = PackageDimensionID;


                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PackageDimension"), objConn, objTran);


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
                if (IsNew) throw new Exception("Delete cannot be performed, PackageDimensionID is missing");
                dicDParam["PackageDimensionID"] = PackageDimensionID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PackageDimension"), objConn, objTran);
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
                     "FROM PackageDimension (NOLOCK) p " +
                     "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND p.PackageDimensionID=" + Database.HandleQuote(PackageDimensionID);
            return Database.HasRows(strSQL);
        }


        public static PackageDimension GetPackageDimension(string CompanyID, PackageDimensionFilter Filter)
        {
            List<PackageDimension> objItems = null;
            PackageDimension objReturn = null;

            try
            {
                objItems = GetPackageDimensions(CompanyID, Filter);
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

        public static List<PackageDimension> GetPackageDimensions(string CompanyID)
        {
            int intTotalCount = 0;
            return GetPackageDimensions(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<PackageDimension> GetPackageDimensions(string CompanyID, PackageDimensionFilter Filter)
        {
            int intTotalCount = 0;
            return GetPackageDimensions(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<PackageDimension> GetPackageDimensions(string CompanyID, PackageDimensionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPackageDimensions(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PackageDimension> GetPackageDimensions(string CompanyID, PackageDimensionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PackageDimension> objReturn = null;
            PackageDimension objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PackageDimension>();

                strSQL = "SELECT p.* " +
                         "FROM PackageDimension (NOLOCK) p " +
                         "WHERE p.CompanyID=" + Database.HandleQuote(CompanyID);

                if (Filter != null)
                {
                    if (Filter.PackageDimensionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageDimensionID, "p.PackageDimensionID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "p.Name");

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PackageDimensionID" : Utility.CustomSorting.GetSortExpression(typeof(PackageDimension), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PackageDimension(objData.Tables[0].Rows[i]);
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
