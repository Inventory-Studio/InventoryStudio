using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class RolePermissionFilter
    {
       
        public Database.Filter.StringSearch.SearchFilter PermissionId { get; set; }
        public Database.Filter.StringSearch.SearchFilter RoleId { get; set; }
    }
}
