using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.Account
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "InActive", Text = "InActive" }
            };
        }
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string UserName { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }


        public List<SelectListItem> StatusOptions { get; set; }

        public List<string>? SelectedRoles { get; set; }

        public List<SelectListItem> AllRoles { get; set; }
    }
}
