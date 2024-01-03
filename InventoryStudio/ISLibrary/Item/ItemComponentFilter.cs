using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemComponentFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemComponentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ChildItemID { get; set; }
        public Database.Filter.NumericSearch.SearchFilter Quantity { get; set; }
    }
}
