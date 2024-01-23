using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.AccessToken
{
    public class AccessTokenViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Application")]
        public string ApplicationName { get; set; }

        [Display(Name = "Token Name")]
        public string TokenName { get; set; }

        public string Token { get; set; }

        public string Secret { get; set; }

        [DisplayName("Inactive")]
        public bool InActive { get; set; }

        public string Role { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }
    }
}
