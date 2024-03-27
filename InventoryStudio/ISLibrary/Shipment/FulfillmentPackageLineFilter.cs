using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public  class FulfillmentPackageLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter FulfillmentPackageID { get; set; }
    }
}
