using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CLRFramework
{
    public partial class Utility
    {
        public class EnumHelper
        {/// <summary>
         /// Retrieve the description on the enum, e.g.
         /// [Description("Bright Pink")]
         /// BrightPink = 2,
         /// Then when you pass in the enum, it will retrieve the description
         /// </summary>
         /// <param name="en">The Enumeration</param>
         /// <returns>A string representing the friendly name</returns>
            public static string GetDescription(Enum en)
            {
                Type type = en.GetType();

                MemberInfo[] memInfo = type.GetMember(en.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return ((DescriptionAttribute)attrs[0]).Description;
                    }
                }

                return en.ToString();
            }

            public static Enum ParseByDescription(Enum en, string EnumDescription)
            {
                Type type = en.GetType();

                MemberInfo[] memInfo = type.GetMember(en.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return null;
                    }
                }

                return null;
            }

            public static List<ExtendedEnum> ToList(Type EnumType)
            {
                if (EnumType == null)
                {
                    throw new ArgumentNullException("type");
                }

                if (!EnumType.IsEnum)
                {
                    throw new Exception("Argument must be of type Enum");
                }

                List<ExtendedEnum> objReturn = new List<ExtendedEnum>();
                string[] arrEnumNames = Enum.GetNames(EnumType);
                Array arrEnumValues = Enum.GetValues(EnumType);
                for (int i = 0; i < arrEnumNames.Length; i++)
                {
                    objReturn.Add(new ExtendedEnum(arrEnumNames[i], Convert.ToString((int)arrEnumValues.GetValue(i)), GetDescription((Enum)arrEnumValues.GetValue(i))));
                }

                //SortedList objSortedList = new SortedList();
                //foreach (Enum enumValue in arrEnumValues)
                //{
                //    objReturn.Add(new ExtendedEnum(enumValue.ToString(), enumValue. Convert.ToString((int)enumValue), GetDescription(enumValue)));
                //    //objSortedList.Add(enumValue, GetDescription(enumValue));
                //}
                return objReturn;
            }
        }

        //[Description("Open")]
        //using System.ComponentModel;
        public class ExtendedEnum
        {
            private string mKey = string.Empty;
            private string mValue = string.Empty;
            private string mDescription = string.Empty;

            public string Key { get { return mKey; } set { mKey = value; } }
            public string Value { get { return mValue; } set { mValue = value; } }
            public string Description { get { return mDescription; } set { mDescription = value; } }

            public ExtendedEnum(string Key, string Value, string Description)
            {
                this.Key = Key;
                this.Value = Value;
                this.Description = Description;
            }
        }
    }
}