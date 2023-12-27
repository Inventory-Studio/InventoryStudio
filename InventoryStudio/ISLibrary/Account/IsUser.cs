
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



    }
}
