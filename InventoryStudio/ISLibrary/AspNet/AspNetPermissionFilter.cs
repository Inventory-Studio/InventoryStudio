using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetPermissionFilter
    {
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }

        public Database.Filter.StringSearch.SearchFilter PermissionId { get; set; }
    }
}
