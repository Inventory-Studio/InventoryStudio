using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class ClientFilter
    {
        public Database.Filter.StringSearch.SearchFilter CompanyName { get; set; }

        public Database.Filter.StringSearch.SearchFilter EmailAddress { get; set; }
    }
}
