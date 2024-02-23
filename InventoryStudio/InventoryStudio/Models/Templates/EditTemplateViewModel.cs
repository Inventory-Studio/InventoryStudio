using System.ComponentModel;

namespace InventoryStudio.Models.Templates
{
    public class EditTemplateViewModel
    {
        public string ImportTemplateID { get; set; }

        [DisplayName("Company ID")]
        public string CompanyID { get; set; }

        [DisplayName("Template Name")]
        public string TemplateName { get; set; }

        public string Type { get; set; }

        [DisplayName("Import Type")]
        public string ImportType { get; set; }
        public List<EditTemplateFieldViewModel> TemplateFields { get; set; }
    }
}
