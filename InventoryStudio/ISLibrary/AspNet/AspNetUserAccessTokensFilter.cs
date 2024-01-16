using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.AspNet
{
    public class AspNetUserAccessTokensFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserAccessTokenId { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserId { get; set; }

    }
}
