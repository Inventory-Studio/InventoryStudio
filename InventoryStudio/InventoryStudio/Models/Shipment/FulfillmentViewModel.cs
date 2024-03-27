using System.ComponentModel;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models
{
    public class FulfillmentViewModel
    {
        public string LocationID { get; set; }
        public string SalesOrderID { get; set; }

        [ValidateNever]
        public List<SalesOrderLine> SalesOrderLines { get; set; }
       
    }
}
