using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class CustomerFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomerID { get; set; }

        public Database.Filter.StringSearch.SearchFilter CreatedBy { get; set; }
    }
}
