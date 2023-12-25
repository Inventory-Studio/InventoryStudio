using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class RoutingProfileLocationFilter
    {
        public Database.Filter.StringSearch.SearchFilter RoutingProfileID { get; set; }
        public Database.Filter.StringSearch.SearchFilter LocationID { get; set; }
    }
}
