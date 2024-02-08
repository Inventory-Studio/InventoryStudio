using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class ImportTemplateFieldFilter
    {
        public Database.Filter.StringSearch.SearchFilter ImportTemplateID { get; set; }

        public Database.Filter.StringSearch.SearchFilter SourceField { get; set; }

        public Database.Filter.StringSearch.SearchFilter DestinationTable { get; set; }

        public Database.Filter.StringSearch.SearchFilter DestinationField { get; set; }
    }
}
