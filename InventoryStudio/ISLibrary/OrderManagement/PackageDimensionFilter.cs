using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class PackageDimensionFilter
    {
        public Database.Filter.StringSearch.SearchFilter PackageDimensionID { get; set; }

        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
    }
}
