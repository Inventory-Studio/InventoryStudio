using System.ComponentModel;

namespace InventoryStudio.Models.Templates
{
    public class TemplateFieldViewModel
    {
        public string ImportTemplateFieldID { get; set; }

        [DisplayName("SourceField")]
        public string SourceField { get; set; } = null!;

        [DisplayName("Destination Table")]
        public string? DestinationTable { get; set; }

        [DisplayName("Destination Field")]
        public string? DestinationField { get; set; }

        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }
    }
}
