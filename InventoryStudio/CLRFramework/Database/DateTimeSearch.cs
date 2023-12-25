using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLRFramework
{
    public partial class Database
    {
        public partial class Filter
        {
            public class DateTimeSearch
            {
                public class SearchFilter
                {
                    public DateTime? From { get; set; }
                    public DateTime? To { get; set; }
                    public SearchOperator? Operator { get; set; }
                }

                public enum SearchOperator
                {
                    between,
                    on,
                    after,
                    before,
                    onOrAfter,
                    onOrBefore
                }

                public static SearchFilter GetSearchFilter(string StringToSet)
                {
                    SearchFilter objReturn = new SearchFilter();

                    try
                    {
                        string strResult = StringToSet.Replace(" ", "");

                        if (strResult.Contains("-"))
                        {
                            objReturn.Operator = SearchOperator.between;
                            strResult = strResult.Replace("-", " ");
                        }
                        else if (strResult.Contains(">="))
                        {
                            objReturn.Operator = SearchOperator.onOrAfter;
                            strResult = strResult.Replace(">=", " ");
                        }
                        else if (strResult.Contains("<="))
                        {
                            objReturn.Operator = SearchOperator.onOrBefore;
                            strResult = strResult.Replace("<=", " ");
                        }
                        else if (strResult.Contains(">"))
                        {
                            objReturn.Operator = SearchOperator.after;
                            strResult = strResult.Replace(">", " ");
                        }
                        else if (strResult.Contains("<"))
                        {
                            objReturn.Operator = SearchOperator.before;
                            strResult = strResult.Replace("<", " ");
                        }
                        else
                        {
                            objReturn.Operator = SearchOperator.on;
                            if (strResult.Contains("=")) strResult = strResult.Replace("=", " ");
                        }

                        string[] strDates = strResult.Split(' ');

                        if (strDates.Length == 2)
                        {
                            if (strDates[0].Trim() == string.Empty)
                            {
                                objReturn.From = Convert.ToDateTime(strDates[1]);
                            }
                            else
                            {
                                objReturn.From = Convert.ToDateTime(strDates[0]);
                                objReturn.To = Convert.ToDateTime(strDates[1]);
                            }
                        }
                        else if (strDates.Length == 1)
                        {
                            objReturn.From = Convert.ToDateTime(strDates[0]);
                        }
                    }
                    catch
                    {
                        objReturn = null;
                    }
                    return objReturn;
                }

                public static string GetSearchFilterText(SearchFilter SearchFilter)
                {
                    string strReturn = string.Empty;

                    if (SearchFilter != null && SearchFilter.Operator != null)
                    {
                        switch (SearchFilter.Operator.Value)
                        {
                            case SearchOperator.on:
                                strReturn = SearchFilter.From.Value.ToShortDateString();
                                break;
                            case SearchOperator.between:
                                strReturn = SearchFilter.From.Value.ToShortDateString() + "-" + SearchFilter.To.Value.ToShortDateString();
                                break;
                            case SearchOperator.after:
                                strReturn = ">" + SearchFilter.From.Value.ToShortDateString();
                                break;
                            case SearchOperator.before:
                                strReturn = "<" + SearchFilter.From.Value.ToShortDateString();
                                break;
                            case SearchOperator.onOrAfter:
                                strReturn = ">=" + SearchFilter.From.Value.ToShortDateString();
                                break;
                            case SearchOperator.onOrBefore:
                                strReturn = "<=" + SearchFilter.From.Value.ToShortDateString();
                                break;
                            default:
                                break;
                        }
                    }
                    return strReturn;
                }

                public static string GetSQLQuery(SearchFilter SearchFilter, string SQLFieldNme)
                {
                    return GetSQLQuery(SearchFilter, SQLFieldNme, false);
                }

                public static string GetSQLQuery(SearchFilter SearchFilter, string SQLFieldNme, bool AllowNull)
                {
                    string strReturn = string.Empty;

                    if (SearchFilter != null && SearchFilter.Operator != null)
                    {
                        switch (SearchFilter.Operator.Value)
                        {
                            case SearchOperator.on:
                                if (AllowNull)
                                    strReturn = "AND (" + SQLFieldNme + " IS NULL OR CAST(FLOOR(CAST(" + SQLFieldNme + " AS FLOAT)) AS DATETIME) = " + Database.HandleQuote(SearchFilter.From.Value.ToString()) + ") ";
                                else
                                    strReturn = "AND CAST(FLOOR(CAST(" + SQLFieldNme + " AS FLOAT)) AS DATETIME) = " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.between:
                                if (AllowNull)
                                    strReturn = "AND (" + SQLFieldNme + " IS NULL OR " + SQLFieldNme + " BETWEEN " + Database.HandleQuote(SearchFilter.From.Value.ToString()) + " AND " + Database.HandleQuote(SearchFilter.To.Value.AddDays(0).ToString()) + ") ";
                                else
                                    strReturn = "AND " + SQLFieldNme + " BETWEEN " + Database.HandleQuote(SearchFilter.From.Value.ToString()) + " AND " + Database.HandleQuote(SearchFilter.To.Value.AddDays(0).ToString());
                                break;
                            case SearchOperator.after:
                                if (AllowNull)
                                    strReturn = "AND (" + SQLFieldNme + " IS NULL OR " + SQLFieldNme + " > " + Database.HandleQuote(SearchFilter.From.Value.AddDays(0).ToString()) + ") ";
                                else
                                    strReturn = "AND " + SQLFieldNme + " > " + Database.HandleQuote(SearchFilter.From.Value.AddDays(0).ToString());
                                break;
                            case SearchOperator.before:
                                if (AllowNull)
                                    strReturn = "AND (" + SQLFieldNme + " IS NULL OR " + SQLFieldNme + " < " + Database.HandleQuote(SearchFilter.From.Value.ToString()) + ") ";
                                else
                                    strReturn = "AND " + SQLFieldNme + " < " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.onOrAfter:
                                if (AllowNull)
                                    strReturn = "AND (" + SQLFieldNme + " IS NULL OR " + SQLFieldNme + " >= " + Database.HandleQuote(SearchFilter.From.Value.ToString()) + ") ";
                                else
                                    strReturn = "AND " + SQLFieldNme + " >= " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.onOrBefore:
                                if (AllowNull)
                                    strReturn = "AND (" + SQLFieldNme + " IS NULL OR " + SQLFieldNme + " <= " + Database.HandleQuote(SearchFilter.From.Value.AddDays(0).ToString()) + ") ";
                                else
                                    strReturn = "AND " + SQLFieldNme + " <= " + Database.HandleQuote(SearchFilter.From.Value.AddDays(0).ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    return strReturn;
                }
            }
        }
    }
}