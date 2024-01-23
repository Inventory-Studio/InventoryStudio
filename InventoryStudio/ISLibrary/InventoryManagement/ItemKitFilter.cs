using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemKitFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemKitID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ChildItemID { get; set; }
        public Database.Filter.NumericSearch.SearchFilter Quantity { get; set; }
    }
}
