using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class InventoryStatusFilter
    {
        public Database.Filter.StringSearch.SearchFilter? CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? Name { get; set; }
    }
}
