using System.ComponentModel;

namespace InventoryStudio.Models.ImportManagement.ImportTemplate
{
    public class EditImportTemplateViewModel
    {
        public string ImportTemplateID { get; set; }

        [DisplayName("Template Name")]
        public string TemplateName { get; set; }

        public string Type { get; set; }

        [DisplayName("Import Type")]
        public string ImportType { get; set; }
        public List<EditImportTemplateFieldViewModel> TemplateFields { get; set; }
    }
}
