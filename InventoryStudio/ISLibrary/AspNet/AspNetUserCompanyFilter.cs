﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLRFramework;

namespace ISLibrary
{ 
    public class AspNetUserCompanyFilter
    {        
        public Database.Filter.StringSearch.SearchFilter UserId { get; set; }
        public Database.Filter.StringSearch.SearchFilter CompanyId { get; set; }

    }
}
