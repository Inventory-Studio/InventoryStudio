using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public  class ItemUnitTypeFilter
    {
        public Database.Filter.StringSearch.SearchFilter CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
    }
}
