using System.ComponentModel;

namespace InventoryStudio.Models.ImportManagement.ImportTemplate
{
    public class EditImportTemplateFieldViewModel
    {
        public string ImportTemplateFieldID { get; set; }

        [DisplayName("Source Field")]
        public string SourceField { get; set; } = null!;

        [DisplayName("Destination Table")]
        public string? DestinationTable { get; set; }

        [DisplayName("Destination Field")]
        public string? DestinationField { get; set; }
    }
}
