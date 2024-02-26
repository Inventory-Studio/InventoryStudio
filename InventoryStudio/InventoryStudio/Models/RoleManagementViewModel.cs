using ISLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models
{
    public class RoleManagementViewModel
    {
        [ValidateNever]
        public AspNetRoles AspNetRoles { get; set; }
        [ValidateNever]
        public AssignUsersViewModel AssignUsersViewModel { get; set; }
        [ValidateNever]
        public AssignPermissionsViewModel AssignPermissionsViewModel { get; set; }

        [ValidateNever]
        public List<AuditData>? AuditDataList { get; set; }

    }
}
