using ISLibrary;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models
{
    public class AssignUsersViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<IsUser> Users { get; set; }
        public List<string> AssignedUserIds { get; set; }
        public List<string> SelectedUserIds { get; set; }
    }
}
