using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class VendorFilter
    {
        public Database.Filter.StringSearch.SearchFilter VendorID { get; set; }
        public Database.Filter.StringSearch.SearchFilter VendorNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter CreatedBy { get; set; }
    }
}
