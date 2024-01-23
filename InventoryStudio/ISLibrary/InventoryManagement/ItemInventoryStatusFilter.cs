using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class ItemInventoryStatusFilter
    {
        public Database.Filter.StringSearch.SearchFilter? CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? InventoryStatusID { get; set; }
    }
}
