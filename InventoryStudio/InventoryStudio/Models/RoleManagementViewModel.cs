using ISLibrary;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models
{
    public class RoleManagementViewModel
    {
        public Role Role { get; set; }
        public AssignUsersViewModel AssignUsersViewModel { get; set; }
        public AssignPermissionsViewModel AssignPermissionsViewModel { get; set; }

    }
}
