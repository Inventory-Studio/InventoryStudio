using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class CompanyFilter
    {
        public Database.Filter.StringSearch.SearchFilter CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentCompanyID { get; set; }

        public Database.Filter.StringSearch.SearchFilter CompanyName { get; set; }
        public Database.Filter.StringSearch.SearchFilter CreatedBy { get; set; }
    }
}
