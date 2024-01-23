using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.AspNet
{
    public class AspNetUserInvitesFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserId { get; set; }

        public Database.Filter.StringSearch.SearchFilter UserInviteId { get; set; }

        public Database.Filter.StringSearch.SearchFilter Code { get; set; }

        public Database.Filter.StringSearch.SearchFilter Email { get; set; }

        public Database.Filter.StringSearch.SearchFilter IsAccepted { get; set; }
    }
}
