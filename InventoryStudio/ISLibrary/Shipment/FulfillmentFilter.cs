using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public  class FulfillmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter FulfillmentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Status { get; set; }
    }
}
