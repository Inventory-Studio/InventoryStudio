
using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class Role : BaseClass
    {
        public string Id { get; set; } = string.Empty;

        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }

        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string ConcurrencyStamp { get; set; } = string.Empty;
        public string Discriminator { get; set; } = "Role";
        public string CompanyId { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;

        private List<int>? mPermissionIds = null;
        public List<int>? AssignedPermissionIds
        {
            get
            {
                if (mPermissionIds == null && !string.IsNullOrEmpty(Id))
                {
                    RolePermissionFilter? objFilter = null;

                    try
                    {
                        mPermissionIds = GetAssignedPermissionIds();
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



        public Role()
        {
        }

        public Role(string Id)
        {
            this.Id = Id;
            Load();
        }

        public Role(DataRow objRow)
        {
            Load(objRow);
        }

        protected void Load()
        {
            base.Load();

            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AspNetRoles (NOLOCK) " +
                         "WHERE Id=" + Database.HandleQuote(Id.ToString());
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
                throw new Exception(ex.Message);
            }
            finally
            {
                objData = null;
            }
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = objRow.Table.Columns;

            try
            {
                if (objColumns.Contains("Id"))  Id = Convert.ToString(objRow["Id"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("NormalizedName")) NormalizedName = Convert.ToString(objRow["NormalizedName"]);
                if (objColumns.Contains("ConcurrencyStamp")) ConcurrencyStamp = objRow["ConcurrencyStamp"].ToString();
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

            try
            {
                // Assume that CompanyID is an identity column and should not be included in the insert.
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (!IsNew) throw new Exception("Create cannot be performed, Id already exists");

                // Add parameters for all the columns in the Company table, except for identity and computed columns.
                dicParam["Name"] = Name;
                dicParam["NormalizedName"] = Name.ToUpperInvariant(); ;
                dicParam["ConcurrencyStamp"] = Guid.NewGuid().ToString();
                dicParam["Discriminator"] = "Role";
                dicParam["CompanyId"] = CompanyId;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["CreatedOn"] = DateTime.UtcNow;

                // Execute the SQL insert and get the new identity value for CompanyID
                Id = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AspNetRoles"), objConn, objTran).ToString();
         


                // Load the newly created company data
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
                if (string.IsNullOrEmpty(Name)) throw new Exception("Name is required");
                if (IsNew) throw new Exception("Update cannot be performed, Id is missing");

                dicParam["Name"] = Name;
                dicParam["NormalizedName"] = Name.ToUpperInvariant();
                dicParam["ConcurrencyStamp"] = Guid.NewGuid().ToString();
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["Id"] = Id;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AspNetRoles"), objConn, objTran);

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

        public static List<Role> GetRoles()
        {
            int intTotalCount = 0;
            return GetRoles(null, null, null, out intTotalCount);
        }

        public static List<Role> GetRoles(RoleFilter Filter)
        {
            int intTotalCount = 0;
            return GetRoles(Filter, null, null, out intTotalCount);
        }

        public static List<Role> GetRoles(RoleFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRoles(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Role> GetRoles(RoleFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Role> objReturn = null;
            Role objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Role>();

                strSQL = "SELECT s.* " +
                         "FROM AspNetRoles (NOLOCK) s ";

                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.CompanyId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CompanyId, "CompanyId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RoutingProfileID" : Utility.CustomSorting.GetSortExpression(typeof(RoutingProfile), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Role(objData.Tables[0].Rows[i]);
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


        public static List<int> GetAssignedPermissionIds()
        {
            List<int> objReturn = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            //try
            //{
            //    strSQL = "SELECT s.PermissionId " +
            //            "FROM AspNetRolePermission (NOLOCK) s " +
            //            " where RoleId = " + Id;

            //    objData = Database.GetDataSet(strSQL);

            //    if (objData != null && objData.Tables[0].Rows.Count > 0)
            //    {
            //        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
            //        {
            //            objReturn.Add(objData.Tables[0].Rows[i]);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    objData = null;
            //}
            return objReturn;
        }



    }
}
