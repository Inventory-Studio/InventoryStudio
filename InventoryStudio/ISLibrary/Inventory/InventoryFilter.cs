using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class InventoryFilter
    {
        public Database.Filter.StringSearch.SearchFilter InventoryID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CartonNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BinID { get; set; }
        public Database.Filter.StringSearch.SearchFilter LotNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentInventoryID { get; set; }
    }
}
