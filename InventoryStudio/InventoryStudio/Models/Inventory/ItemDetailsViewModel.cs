using System.ComponentModel;
using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryStudio.Models;

public class ItemDetailsViewModel : ItemViewModel
{
    [ValidateNever]
    [DisplayName("Product SKU")]
    public string? ProductSku { get; set; }

    [ValidateNever]
    [DisplayName("Product Status")]
    public string? ProductStatus { get; set; }

    [ValidateNever]
    [DisplayName("Product Image")]
    public string? ProductImage { get; set; }

    [ValidateNever]
    [DisplayName("ShipMonk Image")]
    public string? ShipMonkImage { get; set; }

    [ValidateNever]
    [DisplayName("On Hand")]
    public Int32 OnHand { get; set; }

    [ValidateNever]
    [DisplayName("Available")]
    public Int32 Available { get; set; }
}