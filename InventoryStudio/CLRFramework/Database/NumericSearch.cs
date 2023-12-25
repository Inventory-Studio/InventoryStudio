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
            public class NumericSearch
            {
                public class SearchFilter
                {
                    public decimal? From { get; set; }
                    public decimal? To { get; set; }
                    public SearchOperator? Operator { get; set; }
                }

                public enum SearchOperator
                {
                    between,
                    on,
                    greaterThan,
                    lessThan,
                    greaterThanOrEqual,
                    lessThanOrEqual
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
                            objReturn.Operator = SearchOperator.greaterThanOrEqual;
                            strResult = strResult.Replace(">=", " ");
                        }
                        else if (strResult.Contains("<="))
                        {
                            objReturn.Operator = SearchOperator.lessThanOrEqual;
                            strResult = strResult.Replace("<=", " ");
                        }
                        else if (strResult.Contains(">"))
                        {
                            objReturn.Operator = SearchOperator.greaterThan;
                            strResult = strResult.Replace(">", " ");
                        }
                        else if (strResult.Contains("<"))
                        {
                            objReturn.Operator = SearchOperator.lessThan;
                            strResult = strResult.Replace("<", " ");
                        }
                        else
                        {
                            objReturn.Operator = SearchOperator.on;
                            if (strResult.Contains("=")) strResult = strResult.Replace("=", " ");
                        }

                        string[] strNumberss = strResult.Split(' ');

                        if (strNumberss.Length == 2)
                        {
                            if (strNumberss[0].Trim() == string.Empty)
                            {
                                objReturn.From = Convert.ToDecimal(strNumberss[1]);
                            }
                            else
                            {
                                objReturn.From = Convert.ToDecimal(strNumberss[0]);
                                objReturn.To = Convert.ToDecimal(strNumberss[1]);
                            }
                        }
                        else if (strNumberss.Length == 1)
                        {
                            objReturn.From = Convert.ToDecimal(strNumberss[0]);
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
                                strReturn = SearchFilter.From.Value.ToString();
                                break;
                            case SearchOperator.between:
                                strReturn = SearchFilter.From.Value.ToString() + "-" + SearchFilter.To.Value.ToString();
                                break;
                            case SearchOperator.greaterThan:
                                strReturn = ">" + SearchFilter.From.Value.ToString();
                                break;
                            case SearchOperator.lessThan:
                                strReturn = "<" + SearchFilter.From.Value.ToString();
                                break;
                            case SearchOperator.greaterThanOrEqual:
                                strReturn = ">=" + SearchFilter.From.Value.ToString();
                                break;
                            case SearchOperator.lessThanOrEqual:
                                strReturn = "<=" + SearchFilter.From.Value.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                    return strReturn;
                }

                public static string GetSQLQuery(SearchFilter SearchFilter, string SQLFieldNme)
                {
                    string strReturn = string.Empty;

                    if (SearchFilter != null && SearchFilter.Operator != null)
                    {
                        switch (SearchFilter.Operator.Value)
                        {
                            case SearchOperator.on:
                                strReturn = "AND " + SQLFieldNme + " = " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.between:
                                strReturn = "AND (" + SQLFieldNme + " >= " + Database.HandleQuote(SearchFilter.From.Value.ToString()) + " AND " + SQLFieldNme + " <= " + Database.HandleQuote(SearchFilter.To.Value.ToString()) + ") ";
                                break;
                            case SearchOperator.greaterThan:
                                strReturn = "AND " + SQLFieldNme + " > " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.lessThan:
                                strReturn = "AND " + SQLFieldNme + " < " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.greaterThanOrEqual:
                                strReturn = "AND " + SQLFieldNme + " >= " + Database.HandleQuote(SearchFilter.From.Value.ToString());
                                break;
                            case SearchOperator.lessThanOrEqual:
                                strReturn = "AND " + SQLFieldNme + " <= " + Database.HandleQuote(SearchFilter.From.Value.ToString());
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