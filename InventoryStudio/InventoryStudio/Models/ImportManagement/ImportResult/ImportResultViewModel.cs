using System.ComponentModel;

namespace InventoryStudio.Models.ImportManagement.ImportResult
{
    public class ImportResultViewModel
    {
        public string ImportResultID { get; set; }

        [DisplayName("Template Name")]
        public string ImportTemplate { get; set; }

        public int TotalRecords { get; set; }

        public int SuccessfulRecords { get; set; }

        public int FailedRecords { get; set; }

        [DisplayName("Upload By")]
        public string UploadBy { get; set; }

        [DisplayName("Upload Time")]
        public DateTime UploadTime { get; set; }

        [DisplayName("Completion Time")]
        public DateTime? CompletionTime { get; set; }

        public List<ImportFailedRecordViewModel> ImportFailedRecords { get; set; }
    }
}
