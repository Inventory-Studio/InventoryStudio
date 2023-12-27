using ISLibrary;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models
{
    public class AssignPermissionsViewModel
    {
        public string RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public List<Permission> Permissions { get; set; }

        [Display(Name = "Assigned Permissions")]
        public List<string> AssignPermissions { get; set; }

        [Display(Name = "Select Permissions")]
        public List<string> SelectedPermissionIds { get; set; }
    }
}
