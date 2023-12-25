using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryStudio.Models
{
    public class Role : IdentityRole<int>
    {
        //public List<RolePermission> RolePermissions { get; set; }

        public int OrganizationId { get; set; }

        // 导航属性，表示与组织的关系
        //public Organization Organization { get; set; }
    }
}
