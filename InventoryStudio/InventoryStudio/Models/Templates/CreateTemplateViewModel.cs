using System.ComponentModel;

namespace InventoryStudio.Models.Templates
{
    public class CreateTemplateViewModel
    {

        [DisplayName("Company ID")]
        public string CompanyID { get; set; }

        [DisplayName("Template Name")]
        public string TemplateName { get; set; }

        public string Type { get; set; }

        [DisplayName("Import Type")]
        public string ImportType { get; set; }
    }
}
