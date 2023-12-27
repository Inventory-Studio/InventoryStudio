using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryStudio.Models
{


    public class User : IdentityUser<int>
    {


        [MaxLength(10)]
        public string Status { get; set; } = "Active";

        //public int OrganizationId { get; set; }

        [MaxLength(50)]
        public string UserType { get; set; } = "Normal";

        //public ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();

    }
}
