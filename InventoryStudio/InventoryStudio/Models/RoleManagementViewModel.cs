using ISLibrary;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models
{
    public class RoleManagementViewModel
    {
        public AspNetRoles AspNetRoles { get; set; }
        public AssignUsersViewModel AssignUsersViewModel { get; set; }
        public AssignPermissionsViewModel AssignPermissionsViewModel { get; set; }

    }
}
