using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class RoleFilter
    {
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
        public Database.Filter.StringSearch.SearchFilter Id { get; set; }
        public Database.Filter.StringSearch.SearchFilter CompanyId { get; set; }
    }
}
