using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.Client
{
    public class ClientViewModel
    {
        public long ClientId { get; set; }

        public string CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
