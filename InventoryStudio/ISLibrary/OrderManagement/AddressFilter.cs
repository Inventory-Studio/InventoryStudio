using CLRFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class AddressFilter
    {
        public Database.Filter.StringSearch.SearchFilter AddressID { get; set; }

        public Database.Filter.StringSearch.SearchFilter IsInvalidAddress { get; set; }

        public Database.Filter.StringSearch.SearchFilter IsAddressUpdated { get; set; }

        public Database.Filter.StringSearch.SearchFilter FullName { get; set; }

        public Database.Filter.StringSearch.SearchFilter Email { get; set; }

        public Database.Filter.StringSearch.SearchFilter CreatedBy { get; set; }

    }
}
