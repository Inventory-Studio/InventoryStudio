using System.ComponentModel;

namespace InventoryStudio.Models.Templates
{
    public class TemplateViewModel
    {
        public string ImportTemplateID { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Template Name")]
        public string TemplateName { get; set; }

        public string Type { get; set; }

        [DisplayName("Import Type")]
        public string ImportType { get; set; }

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
