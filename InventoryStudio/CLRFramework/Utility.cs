using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLRFramework
{
    public partial class Utility
    {
        public static bool IsValidPositiveDollarAmount(string strAmount)
        {
            Regex reg = new Regex(@"^\d+(\.\d{1,2})?$");
            return reg.IsMatch(strAmount);
        }

        public static decimal GetRoundedAmount(decimal dcmAmount)
        {
            return Math.Round(dcmAmount, 2, MidpointRounding.AwayFromZero);
        }

        public static double GetRoundedAmount(double dbAmount)
        {
            return Math.Round(dbAmount, 2, MidpointRounding.AwayFromZero);
        }

        public static bool IsInteger(string strParam)
        {
            int intReturn = 0;
            return int.TryParse(strParam, out intReturn);
        }

        public static bool IsDecimal(string strParam)
        {
            decimal dcmReturn = 0;
            return decimal.TryParse(strParam, out dcmReturn);
        }

        public static bool IsDouble(string strParam)
        {
            double dbReturn = 0;
            return double.TryParse(strParam, out dbReturn);
        }

        public static bool IsBoolean(string strParam)
        {
            bool blnReturn = false;
            if (strParam == "1")
                return true;
            else if (strParam == "0")
                return false;
            else
                return bool.TryParse(strParam, out blnReturn);
        }

        public string BreakLongWord(string StringToBreak, string SymbolToInsert)
        {
            if (string.IsNullOrEmpty(StringToBreak))
            {
                return string.Empty;
            }
            string pattern = @"(\S{20})(\S)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.Replace(StringToBreak, "$1-$2");
            //return regex.Replace(StringToBreak, "$1&#8203;$2"); //for zero width space
            //return regex.Replace(StringToBreak, @"$1 $2"); //for space...or use "$1<wbr>$2"
        }

        public static string GetFirstNWords(string InputString, int NumberOfWords)
        {
            bool blnHasMoreThanNWords;
            return GetFirstNWords(InputString, NumberOfWords, out blnHasMoreThanNWords);
        }

        public static string GetFirstNWords(string InputString, int NumberOfWords, string StringToAppenIfMoreThanNWords)
        {
            bool blnHasMoreThanNWords;
            return GetFirstNWords(InputString, NumberOfWords, StringToAppenIfMoreThanNWords, out blnHasMoreThanNWords);
        }

        public static string GetFirstNWords(string InputString, int NumberOfWords, out bool HasMoreThanNWords)
        {
            return GetFirstNWords(InputString, NumberOfWords, "", out HasMoreThanNWords);
        }

        public static string GetFirstNWords(string InputString, int NumberOfWords, string StringToAppenIfHasMoreThanNWords, out bool HasMoreThanNWords)
        {
            string strReturn = string.Empty;
            HasMoreThanNWords = false;

            if (!string.IsNullOrEmpty(InputString))
            {
                if (InputString.Split(' ').Length > NumberOfWords)
                {
                    HasMoreThanNWords = true;
                    strReturn = String.Join(" ", InputString.Split(' '), 0, NumberOfWords) + StringToAppenIfHasMoreThanNWords;
                }
                else
                {
                    HasMoreThanNWords = false;
                    strReturn = InputString;
                }
            }
            return strReturn;
        }
    }
}
