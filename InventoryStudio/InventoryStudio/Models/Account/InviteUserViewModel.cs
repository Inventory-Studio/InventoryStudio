using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.Account
{
    public class InviteUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
