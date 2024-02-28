using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public  class AdjustmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter LocationID { get; set; }
    }
}
