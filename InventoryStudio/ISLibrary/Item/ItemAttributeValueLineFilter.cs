using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLRFramework;

namespace ISLibrary
{
    public class ItemAttributeValueLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemAttributeValueID { get; set; }
    }
}
