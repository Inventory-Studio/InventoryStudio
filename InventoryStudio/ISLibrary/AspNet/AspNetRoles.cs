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

namespace ISLibrary
{
    public class AspNetRoles : BaseClass
    {
        public string Id { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string ConcurrencyStamp { get; set; } = string.Empty;
        public string Discriminator { get; set; } = "Role";
        public string CompanyId { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        private List<string>? mUserIds = null;
        public List<string>? AssignUserIds
        {
            get
            {
                if (mUserIds == null && !string.IsNullOrEmpty(Id))
                {

                    try
                    {
                        mUserIds = GetAssignUserIds();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                }
                return mUserIds;
            }
        }

        private List<string>? mPermissionIds = null;
        public List<string>? AssignPermissionIds
        {
            get
            {
                if (mPermissionIds == null && !string.IsNullOrEmpty(Id))
                {
                    AspNetRolePermissionFilter? objFilter = null;

                    try
                    {
                        mPermissionIds = GetAssignPermissionIds();
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
                return mPermissionIds;
            }
        }

        private List<AspNetPermission>? mPermissions = null;
        public List<AspNetPermission>? AssignPermissions
        {
            get
            {
                if (mPermissions == null && !string.IsNullOrEmpty(Id))
                {
                    try
                    {
                        mPermissions = GetAssignPermissions();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                return mPermissions;
            }
        }

        public AspNetRoles()
        {
        }

        public AspNetRoles(/*string CompanyID,*/ string Id)
        {
            //this.CompanyID = CompanyID;
            this.Id = Id;
            this.Load();
        }

        public AspNetRoles(DataRow objRow)
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
                         "FROM AspNetRoles (NOLOCK) " +
                         "WHERE Id=" + Database.HandleQuote(Id);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Id=" + Id + " is not found");
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

                if (objColumns.Contains("Id")) Id = Convert.ToString(objRow["Id"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("NormalizedName")) NormalizedName = Convert.ToString(objRow["NormalizedName"]);
                if (objColumns.Contains("ConcurrencyStamp")) ConcurrencyStamp = Convert.ToString(objRow["ConcurrencyStamp"]);
                if (objColumns.Contains("Discriminator")) Discriminator = Convert.ToString(objRow["Discriminator"]);
                if (objColumns.Contains("CompanyId")) CompanyId = Convert.ToString(objRow["CompanyId"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(Id)) throw new Exception("Missing Id in the datarow");
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
                //if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                //if (string.IsNullOrEmpty(AspNetRolesName)) throw new Exception("AspNetRolesName is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Name"] = Name;
                dicParam["NormalizedName"] = NormalizedName;
                dicParam["ConcurrencyStamp"] = ConcurrencyStamp;
                dicParam["Discriminator"] = Discriminator;
                dicParam["CompanyId"] = CompanyId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                Id = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetRoles"), objConn, objTran).ToString();


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
                //if (string.IsNullOrEmpty(CompanyID)) throw new Exception("CompanyID is required");
                //if (string.IsNullOrEmpty(AspNetRolesName)) throw new Exception("AspNetRolesName is required");
                if (IsNew) throw new Exception("Update cannot be performed, CompanyUserID is missing");
                //if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Name"] = Name;
                dicParam["NormalizedName"] = NormalizedName;
                dicParam["ConcurrencyStamp"] = ConcurrencyStamp;
                dicParam["Discriminator"] = Discriminator;
                dicParam["CompanyId"] = CompanyId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["Id"] = Id;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetRoles"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, Id is missing");

                dicDParam["Id"] = Id;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AspNetRoles"), objConn, objTran);
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
                     "FROM AspNetRoles (NOLOCK) p " +
                     "WHERE 1=1 " +
                     "AND p.CompanyId=" + Database.HandleQuote(CompanyId) +
                     "AND p.Name=" + Database.HandleQuote(Name);

            if (!string.IsNullOrEmpty(Id)) strSQL += "AND p.Id<>" + Database.HandleQuote(Id);
            return Database.HasRows(strSQL);
        }

        public static AspNetRoles GetAspNetRoles(string CompanyID, AspNetRolesFilter Filter)
        {
            List<AspNetRoles> objAspNetRoless = null;
            AspNetRoles objReturn = null;

            try
            {
                objAspNetRoless = GetAspNetRoless(CompanyID, Filter);
                if (objAspNetRoless != null && objAspNetRoless.Count >= 1) objReturn = objAspNetRoless[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAspNetRoless = null;
            }
            return objReturn;
        }

        public static List<AspNetRoles> GetAspNetRoless(string CompanyID)
        {
            int intTotalCount = 0;
            return GetAspNetRoless(CompanyID, null, null, null, out intTotalCount);
        }

        public static List<AspNetRoles> GetAspNetRoless(string CompanyID, AspNetRolesFilter Filter)
        {
            int intTotalCount = 0;
            return GetAspNetRoless(CompanyID, Filter, null, null, out intTotalCount);
        }

        public static List<AspNetRoles> GetAspNetRoless(string CompanyID, AspNetRolesFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAspNetRoless(CompanyID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AspNetRoles> GetAspNetRoless(string CompanyID, AspNetRolesFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AspNetRoles> objReturn = null;
            AspNetRoles objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AspNetRoles>();

                strSQL = "SELECT * " +
                         "FROM AspNetRoles (NOLOCK) " +
                         "WHERE CompanyID=" + Database.HandleQuote(CompanyID);
                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Id" : Utility.CustomSorting.GetSortExpression(typeof(AspNetRoles), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetRoles(objData.Tables[0].Rows[i]);
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

        public List<string> GetAssignUserIds()
        {
            List<string> objReturn = new List<string>(); ;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.UserId " +
                        "FROM AspNetUserRoles (NOLOCK) s " +
                        " where RoleId = " + Id;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        DataRow objRow = objData.Tables[0].Rows[i];
                        DataColumnCollection objColumns = objRow.Table.Columns;
                        if (objColumns.Contains("UserId"))
                        {
                            objReturn.Add(Convert.ToString(objRow["UserId"]));
                        }

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

        public List<string> GetAssignPermissionIds()
        {
            List<string> objReturn = new List<string>(); ;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.PermissionId " +
                        "FROM AspNetRolePermission (NOLOCK) s " +
                        " where RoleId = " + Id;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        DataRow objRow = objData.Tables[0].Rows[i];
                        DataColumnCollection objColumns = objRow.Table.Columns;
                        if (objColumns.Contains("PermissionId"))
                        {
                            objReturn.Add(Convert.ToString(objRow["PermissionId"]));
                        }

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

        public List<AspNetPermission> GetAssignPermissions()
        {
            List<AspNetPermission> objReturn = new List<AspNetPermission>(); ;
            AspNetPermission objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT s.* " +
                        "FROM AspNetPermission (NOLOCK) s " +
                        "INNER JOIN  AspNetRolePermission ap on ap.PermissionId = s.PermissionId" +
                        " where ap.RoleId = " + Id;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AspNetPermission(objData.Tables[0].Rows[i]);
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
