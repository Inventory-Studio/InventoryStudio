using System.ComponentModel;
using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models
{
    public class TransferViewModel
    {
        public string Memo { get; set; }

        [ValidateNever]
        public List<TransferLine> TransferLines { get; set; }
       
    }
}
