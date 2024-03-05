using CLRFramework;
using ISLibrary.OrderManagement;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class LabelProfile : BaseClass
    {
        public string LabelProfileID { get; set; }

        public bool IsNew { get { return string.IsNullOrEmpty(LabelProfileID); } }

        public string CompanyID { get; set; }

        public string ProfileName { get; set; } = null!;

        public string? PrintNodePrinterID { get; set; }

        public string? Width { get; set; }

        public string? Height { get; set; }

        public bool? IsTextVisible { get; set; }

        public string? SVGMode { get; set; }

        public string? MarginLeft { get; set; }

        public string? MarginRight { get; set; }

        public string? MarginTop { get; set; }

        public string? MarginBottom { get; set; }

        public string? TextPropertyTop { get; set; }

        public string? TextPropertyBottom { get; set; }

        public string? TextPosition { get; set; }

        public string? Textalignment { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        public LabelProfile()
        {

        }

        public LabelProfile(string CompanyID)
        {
            this.CompanyID = CompanyID;
            Load();
        }

        public LabelProfile(string CompanyID, string LabelProfileID)
        {
            this.CompanyID = CompanyID;
            this.LabelProfileID = LabelProfileID;
            Load();
        }

        public LabelProfile(DataRow row)
        {
            Load(row);
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;
            try
            {
                objColumns = objRow.Table.Columns;
                if (objColumns.Contains("LabelProfileID")) LabelProfileID = Convert.ToString(objRow["LabelProfileID"]);
                if (objColumns.Contains("CompanyID")) CompanyID = Convert.ToString(objRow["CompanyID"]);
                if (objColumns.Contains("ProfileName")) ProfileName = Convert.ToString(objRow["ProfileName"]);
                if (objColumns.Contains("PrintNodePrinterID")) PrintNodePrinterID = Convert.ToString(objRow["PrintNodePrinterID"]);
                if (objColumns.Contains("Width")) Width = Convert.ToString(objRow["Width"]);
                if (objColumns.Contains("Height")) Height = Convert.ToString(objRow["Height"]);
                if (objColumns.Contains("IsTextVisible") && objRow["IsTextVisible"] != DBNull.Value) IsTextVisible = Convert.ToBoolean(objRow["IsTextVisible"]);
                if (objColumns.Contains("SVGMode")) SVGMode = Convert.ToString(objRow["SVGMode"]);
                if (objColumns.Contains("MarginLeft")) MarginLeft = Convert.ToString(objRow["MarginLeft"]);
                if (objColumns.Contains("MarginRight")) MarginRight = Convert.ToString(objRow["MarginRight"]);
                if (objColumns.Contains("MarginTop")) MarginTop = Convert.ToString(objRow["MarginTop"]);
                if (objColumns.Contains("MarginBottom")) MarginBottom = Convert.ToString(objRow["MarginBottom"]);
                if (objColumns.Contains("TextPropertyTop")) TextPropertyTop = Convert.ToString(objRow["TextPropertyTop"]);
                if (objColumns.Contains("TextPropertyBottom")) TextPropertyBottom = Convert.ToString(objRow["TextPropertyBottom"]);
                if (objColumns.Contains("TextPosition")) TextPosition = Convert.ToString(objRow["TextPosition"]);
                if (objColumns.Contains("Textalignment")) Textalignment = Convert.ToString(objRow["Textalignment"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
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
            base.Load();
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                strSQL = "SELECT l.* " +
                         "FROM LabelProfileID v (NOLOCK) " +
                         "WHERE l.CompanyID=" + Database.HandleQuote(CompanyID) +
                         "AND l.LabelProfileID = " + Database.HandleQuote(LabelProfileID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("LabelProfile is not found");
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

        public bool Create()
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

        public bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();
            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ProfileName)) throw new Exception("ProfileName is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, LabelProfileID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["ProfileName"] = ProfileName;
                dicParam["PrintNodePrinterID"] = PrintNodePrinterID;
                dicParam["Width"] = Width;
                dicParam["Height"] = Height;
                dicParam["IsTextVisible"] = IsTextVisible;
                dicParam["SVGMode"] = SVGMode;
                dicParam["MarginLeft"] = MarginLeft;
                dicParam["MarginRight"] = MarginRight;
                dicParam["MarginTop"] = MarginTop;
                dicParam["MarginBottom"] = MarginBottom;
                dicParam["TextPropertyTop"] = TextPropertyTop;
                dicParam["TextPropertyBottom"] = TextPropertyBottom;
                dicParam["TextPosition"] = TextPosition;
                dicParam["Textalignment"] = Textalignment;
                dicParam["CreatedBy"] = CreatedBy;
                dicParam["CreatedOn"] = DateTime.Now;
                LabelProfileID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "LabelProfile"), objConn, objTran).ToString();
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

        public bool Update()
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

        public bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                if (string.IsNullOrEmpty(ProfileName)) throw new Exception("ProfileName is required");
                if (string.IsNullOrEmpty(UpdatedBy)) throw new Exception("UpdatedBy is required");
                if (IsNew) throw new Exception("Create cannot be performed, LabelProfileID already exists");
                if (!ObjectAlreadyExists()) throw new Exception("This record already exists");
                dicParam["CompanyID"] = CompanyID;
                dicParam["ProfileName"] = ProfileName;
                dicParam["PrintNodePrinterID"] = PrintNodePrinterID;
                dicParam["Width"] = Width;
                dicParam["Height"] = Height;
                dicParam["IsTextVisible"] = IsTextVisible;
                dicParam["SVGMode"] = SVGMode;
                dicParam["MarginLeft"] = MarginLeft;
                dicParam["MarginRight"] = MarginRight;
                dicParam["MarginTop"] = MarginTop;
                dicParam["MarginBottom"] = MarginBottom;
                dicParam["TextPropertyTop"] = TextPropertyTop;
                dicParam["TextPropertyBottom"] = TextPropertyBottom;
                dicParam["TextPosition"] = TextPosition;
                dicParam["Textalignment"] = Textalignment;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.Now;
                dicWParam["LabelProfileID"] = LabelProfileID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "LabelProfile"), objConn, objTran);
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
                if (string.IsNullOrEmpty(LabelProfileID)) throw new Exception("Delete cannot be performed, CustomerID is missing");
                dicDParam["LabelProfileID"] = LabelProfileID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "LabelProfile"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            LogAuditData(enumActionType.Delete);
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;
            strSQL = "SELECT TOP 1 l.* " +
                     "FROM LabelProfile (NOLOCK) l " +
                     "WHERE l.CompanyID=" + Database.HandleQuote(CompanyID) +
                     "AND l.ProfileName=" + Database.HandleQuote(ProfileName);
            return Database.HasRows(strSQL);
        }

        public static List<LabelProfile> GetLabelProfiles(string CompanyID)
        {
            int intTotalCount = 0;
            return GetLabelProfiles(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<LabelProfile> GetLabelProfiles(string CompanyID, LabelProfileFilter Filter)
        {
            int intTotalCount = 0;
            return GetLabelProfiles(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<LabelProfile> GetLabelProfiles(string CompanyID, LabelProfileFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetLabelProfiles(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<LabelProfile> GetLabelProfiles(string CompanyID, LabelProfileFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<LabelProfile> objReturn = null;
            LabelProfile objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;
            try
            {
                TotalRecord = 0;
                objReturn = new List<LabelProfile>();
                strSQL = "SELECT l.* " +
                                         "FROM LabelProfile (NOLOCK) l " +
                                         "WHERE l.CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.LabelProfileID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LabelProfileID, "l.LabelProfileID");
                    if (Filter.ProfileName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ProfileName, "l.ProfileName");
                }
                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "LabelProfileID" : Utility.CustomSorting.GetSortExpression(typeof(LabelProfile), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new LabelProfile(objData.Tables[0].Rows[i]);
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
