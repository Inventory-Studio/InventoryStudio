using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLRFramework;

namespace ISLibrary
{
    public class ItemAttributeValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemAttributeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AttributeValueName { get; set; }
    }
}
