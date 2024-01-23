using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLRFramework;

namespace ISLibrary
{
    public class ItemAttributeFilter
    {
        public Database.Filter.StringSearch.SearchFilter ClientID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AttributeName { get; set; }
    }
}
