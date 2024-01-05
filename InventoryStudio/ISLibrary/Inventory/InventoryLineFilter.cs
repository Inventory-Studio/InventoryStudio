using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class InventoryLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter InventoryLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InventoryID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentInventoryLineID { get; set; }
    }
}
