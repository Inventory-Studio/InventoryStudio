using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class SalesOrderLineAllocationFilter
    {
        public Database.Filter.StringSearch.SearchFilter? CompanyID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? SalesOrderLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? LocationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? InboundShipmentLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? PurchaseOrderLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? TransferOrderLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter? WorkOrderLineID { get; set; }
        public Database.Filter.NumericSearch.SearchFilter? QuantityAllocated { get; set; }
    }
}
