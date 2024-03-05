using CLRFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class LabelProfileFilter
    {
        public Database.Filter.StringSearch.SearchFilter LabelProfileID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ProfileName { get; set; }

    }
}
