using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemMatrixFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
    }
}
