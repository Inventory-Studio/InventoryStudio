using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.OrderManagement.Client
{
    public class CreateClientViewModel
    {
        [Required]
        [Display(Name = "Company")]
        public string CompanyId { get; set; }

        [Required]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; } = null!;

        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("E-mail")]
        public string EmailAddress { get; set; } = null!;
    }
}
