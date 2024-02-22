using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models
{
    public class ItemViewModel
    {
        [ValidateNever]
        public Item Item { get; set; }
        [ValidateNever]
        public ItemParent ItemParent { get; set; }
        [ValidateNever]
        public List<ItemAttribute> ItemAttributes { get; set; }
        [ValidateNever]
        public List<ItemMatrix> ItemMatrices { get; set; }


    }
}
