using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CLRFramework
{
    public partial class Utility
    {
        public class CustomSorting
        {
            [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
            public class SqlSortingFieldAttribute : Attribute
            {
                public string FieldName { get; set; }
                public bool IsMultiValue { get; set; }
                public SqlSortingFieldAttribute(string fieldName)
                {
                    this.FieldName = fieldName;
                }
            }

            public static string GetSortExpression(Type ObjectType, string PropertyName)
            {
                string strReturn = PropertyName;

                foreach (var prop in PropertyName.Split('.').Select(s => ObjectType.GetProperty(s)))
                {
                    if (prop != null)
                    {
                        if (prop.PropertyType != typeof(System.String))
                        {
                            ObjectType = prop.PropertyType;
                        }
                        else
                        {
                            PropertyName = prop.Name;
                            strReturn = PropertyName;
                        }
                    }
                }

                System.Reflection.PropertyInfo objProperty = ObjectType.GetProperty(PropertyName);
                if (objProperty != null)
                {
                    object[] attrs = objProperty.GetCustomAttributes(typeof(Utility.CustomSorting.SqlSortingFieldAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        strReturn = ((Utility.CustomSorting.SqlSortingFieldAttribute)attrs[0]).FieldName;
                    }
                }
                return strReturn;
            }
        }
    }
}