using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.AspNet
{
    public class AspNetAccessTokensFilter
    {
        public Database.Filter.StringSearch.SearchFilter AccessTokenID { get; set; }
        public Database.Filter.StringSearch.SearchFilter RoleId { get; set; }

    }
}
