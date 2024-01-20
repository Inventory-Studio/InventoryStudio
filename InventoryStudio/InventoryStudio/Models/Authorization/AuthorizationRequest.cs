using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.Authorization
{
    public class AuthorizationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
