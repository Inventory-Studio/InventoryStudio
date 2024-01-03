﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetRoles : BaseClass
    {
        public string Id { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Discriminator { get; set; }
        public string CompanyId { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }


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
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

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
                     "AND p.CompanyId=" + Database.HandleQuote(CompanyId);

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
    }
}
