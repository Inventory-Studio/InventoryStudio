using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.AccessToken
{
    public class CreateAccessTokenViewModel
    {
        [Required]
        [MaxLength(256)]
        [Display(Name = "Application")]
        public string ApplicationName { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Token Name")]
        public string TokenName { get; set; }

        public bool InActive { get; set; }

        [Required]
        [MaxLength(450)]
        [Display(Name = "Role")]
        public string RoleId { get; set; }
    }
}
