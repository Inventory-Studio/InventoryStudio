using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetUserTokensFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserId { get; set; }
        public Database.Filter.StringSearch.SearchFilter LoginProvider { get; set; }

    }
}
