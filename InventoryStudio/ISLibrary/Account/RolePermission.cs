
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
    public class RolePermission : BaseClass
    {
        public string PermissionId { get; set; } = string.Empty;


        public string RoleId { get; set; } = string.Empty;


        public RolePermission()
        {
        }

        public RolePermission(string Id)
        {
            this.PermissionId = Id;
            Load();
        }

        public RolePermission(DataRow objRow)
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
                         "FROM AspNetRolePermission (NOLOCK) " +
                         "WHERE PermissionId=" + Database.HandleQuote(PermissionId.ToString());
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PermissionId=" + PermissionId + " is not found");
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
                if (objColumns.Contains("PermissionId")) PermissionId = Convert.ToString(objRow["PermissionId"]);
                if (objColumns.Contains("RoleId")) RoleId = Convert.ToString(objRow["RoleId"]);               

                if (string.IsNullOrEmpty(PermissionId)) throw new Exception("Missing PermissionId in the datarow");
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


        public static List<Permission> GetPermissions()
        {
            int intTotalCount = 0;
            return GetPermissions(null, null, null, out intTotalCount);
        }

        public static List<Permission> GetPermissions(PermissionFilter Filter)
        {
            int intTotalCount = 0;
            return GetPermissions(Filter, null, null, out intTotalCount);
        }

        public static List<Permission> GetPermissions(PermissionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPermissions(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Permission> GetPermissions(PermissionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Permission> objReturn = null;
            Permission objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Permission>();

                strSQL = "SELECT s.* " +
                         "FROM AspNetPermission (NOLOCK) s ";

                strSQL += "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.PermissionId != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PermissionId, "PermissionId");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PermissionID" : Utility.CustomSorting.GetSortExpression(typeof(Permission), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Permission(objData.Tables[0].Rows[i]);
                        //objNew.IsLoaded = true;
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
