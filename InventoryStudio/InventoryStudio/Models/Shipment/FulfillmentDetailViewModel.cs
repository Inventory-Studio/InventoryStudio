using System.ComponentModel;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models
{
    public class FulfillmentDetailViewModel
    {

        [ValidateNever]
        public Fulfillment Fulfillment { get; set; }
       
    }
}
