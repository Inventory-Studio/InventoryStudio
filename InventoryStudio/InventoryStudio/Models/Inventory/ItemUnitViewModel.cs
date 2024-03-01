using System.ComponentModel;
using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models
{
    public class ItemUnitViewModel
    {
        public string Name { get; set; }

        [ValidateNever]
        public List<ItemUnit> ItemUnits { get; set; }
       
    }
}
