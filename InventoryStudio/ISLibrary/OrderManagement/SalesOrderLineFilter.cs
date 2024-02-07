using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class SalesOrderLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter SalesOrderLineID { get; set; }

        public Database.Filter.StringSearch.SearchFilter SalesOrderID { get; set; }
    }
}
