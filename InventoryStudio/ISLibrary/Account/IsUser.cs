
using CLRFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    //because of EF core already have User, so this model add prefix Is
    public class IsUser : BaseClass
    {
        public string Id { get; set; } = string.Empty;

        public bool IsNew { get { return string.IsNullOrEmpty(Id); } }

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;



        private List<Company>? mCompanies = null;
        public List<Company>? Companies
        {
            get
            {
                if (mCompanies == null && !string.IsNullOrEmpty(Id))
                {           

                    try
                    {
                        mCompanies = GetCompanies(Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        
                    }
                }
                return mCompanies;
            }
        } 
        
        private List<Role>? mRoles = null;
        public List<Role>? Roles
        {
            get
            {
                if (mRoles == null && !string.IsNullOrEmpty(Id))
                {           

                    try
                    {
                        mRoles = GetRoles(Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        
                    }
                }
                return mRoles;
            }
        }

        public IsUser(DataRow objRow)
        {
            Load(objRow);
        }


        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = objRow.Table.Columns;

            try
            {
                if (objColumns.Contains("Id")) Id = Convert.ToString(objRow["Id"]);
                if (objColumns.Contains("UserName")) UserName = Convert.ToString(objRow["UserName"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
               
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

        public IsUser()
        {
        }

        public IsUser(string Id)
        {
            this.Id = Id;
            Load();
        }


        protected void Load()
        {
            base.Load();

            DataSet? objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AspNetUsers (NOLOCK) " +
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

        public static List<IsUser> GetSameCompanyUsers(string compayId)
        {
            List<IsUser> objReturn = new List<IsUser>();
            IsUser objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT u.* "+
                    "FROM AspNetUsers u "+
                    "INNER JOIN AspNetUserCompany uc ON u.Id = uc.UserId "+
                    "WHERE uc.CompanyId = " + compayId;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new IsUser(objData.Tables[0].Rows[i]);
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


        public static List<Company> GetCompanies(string userId)
        {
            List<Company> objReturn = new List<Company>();
            Company objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT u.* " +
                    "FROM Company u " +
                    "INNER JOIN AspNetUserCompany uc ON u.CompanyID = uc.CompanyId " +
                    "WHERE uc.UserId = " + userId;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Company(objData.Tables[0].Rows[i]);
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

        public static List<Role> GetRoles(string userId)
        {
            List<Role> objReturn = new List<Role>();
            Role objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT u.* " +
                    "FROM AspNetRoles u " +
                    "INNER JOIN AspNetUserRoles uc ON u.Id = uc.RoleId " +
                    "WHERE uc.UserId = " + userId;

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Role(objData.Tables[0].Rows[i]);
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
