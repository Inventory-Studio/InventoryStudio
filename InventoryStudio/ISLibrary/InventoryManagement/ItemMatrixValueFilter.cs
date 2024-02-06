using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemMatrixValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemMatrixID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemAttributeValueID { get; set; }
    }
}
