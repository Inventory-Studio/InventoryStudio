using CLRFramework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class SalesOrderFilter
    {
        public Database.Filter.StringSearch.SearchFilter PONumber { get; set; }

    }
}
