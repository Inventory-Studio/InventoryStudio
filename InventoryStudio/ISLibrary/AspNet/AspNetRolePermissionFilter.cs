using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetRolePermissionFilter
    {
        public Database.Filter.StringSearch.SearchFilter RoleId { get; set; }
        public Database.Filter.StringSearch.SearchFilter PermissionId { get; set; }
    }
}
