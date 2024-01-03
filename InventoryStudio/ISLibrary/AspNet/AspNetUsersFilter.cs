using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{
    public class AspNetUsersFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomerID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PONumber { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter TranDate { get; set; }
        public Database.Filter.StringSearch.SearchFilter LocationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShipToAddressID { get; set; }
    }
}
