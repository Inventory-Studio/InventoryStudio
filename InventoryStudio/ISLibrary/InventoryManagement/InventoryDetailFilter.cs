using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public  class InventoryDetailFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter LocationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BinID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InventoryNumber { get; set; }
    }
}
