using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemUnitFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemUnitTypeID { get; set; }

        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
    }
}
