using System.ComponentModel;

namespace InventoryStudio.Models.ImportManagement.ImportResult
{
    public class ImportFailedRecordViewModel
    {

        [DisplayName("Error Message")]
        public string? ErrorMessage { get; set; }

        [DisplayName("Failed Data")]
        public string? FailedData { get; set; }
    }
}
