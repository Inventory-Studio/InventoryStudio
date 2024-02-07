using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class LocationFilter
    {
        public Database.Filter.StringSearch.SearchFilter LocationID { get; set; }

        public Database.Filter.StringSearch.SearchFilter ParentLocationID { get; set; }
    }
}
