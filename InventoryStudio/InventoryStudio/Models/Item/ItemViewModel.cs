using ISLibrary;

namespace InventoryStudio.Models
{
    public class ItemViewModel
    {
      
        public Item Item { get; set; }
        public ItemParent ItemParent { get; set; }

        public List<ItemAttribute> ItemAttributes { get; set; }
        public List<ItemMatrix> ItemMatrices { get; set; }


    }
}
