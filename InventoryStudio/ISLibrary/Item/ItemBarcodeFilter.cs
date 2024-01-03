using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class ItemBarcodeFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemBarcodeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Barcode { get; set; }
        public Database.Filter.StringSearch.SearchFilter Type { get; set; }
    }
}
