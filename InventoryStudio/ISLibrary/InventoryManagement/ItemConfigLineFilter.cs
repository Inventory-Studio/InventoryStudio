using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class ItemConfigLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter? CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? ItemConfigID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? ItemComponentID { get; set; }
        public Database.Filter.NumericSearch.SearchFilter? Quantity { get; set; }
    }
}
