using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class BinFilter
    {
        public Database.Filter.StringSearch.SearchFilter BinID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BinNumber { get; set; }
    }
}
