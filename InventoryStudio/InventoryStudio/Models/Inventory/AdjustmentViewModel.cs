using System.ComponentModel;
using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models
{
    public class AdjustmentViewModel
    {
        public string Memo { get; set; }
        public string LocationID { get; set; }

        [ValidateNever]
        public List<AdjustmentLine> AdjustmentLines { get; set; }
       
    }
}
