using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLRFramework;

namespace ISLibrary
{
    public class ItemFilter
    {
        public Database.Filter.StringSearch.SearchFilter ClientID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemParentID { get; set; }
        //public List<Item.enumItemType> ItemTypes { get; set; }
        public List<string> ItemTypes { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemName { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesDescription { get; set; }
        public Database.Filter.StringSearch.SearchFilter PurchaseDescription { get; set; }
        public Database.Filter.StringSearch.SearchFilter Barcode { get; set; }
        public bool? IsBarcoded { get; set; }
        public bool? IsShipReceiveIndividually { get; set; }
        public bool? DisplayComponents { get; set; }
    }
}
