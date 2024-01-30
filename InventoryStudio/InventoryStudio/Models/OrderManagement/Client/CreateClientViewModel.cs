using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.Client
{
    public class CreateClientViewModel
    {
        [Required]
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = null!;
    }
}
