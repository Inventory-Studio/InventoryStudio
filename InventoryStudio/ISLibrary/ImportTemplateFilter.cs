using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class ImportTemplateFilter
    {
        public Database.Filter.StringSearch.SearchFilter TemplateName { get; set; }

        public Database.Filter.StringSearch.SearchFilter Type { get; set; }

        public Database.Filter.StringSearch.SearchFilter ImportType { get; set; }
    }
}
