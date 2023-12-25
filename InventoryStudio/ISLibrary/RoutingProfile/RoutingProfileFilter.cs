using CLRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class RoutingProfileFilter
    {
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
        public List<Routing.enumFulfillmentMethod> FulfillmentMethods { get; set; }
        public List<Routing.enumFulfillmentStrategy> FulfillmentStrategies { get; set; }
        public List<Routing.enumAllocationStrategy> AllocationStrategies { get; set; }
        public bool? ApprovalRequired { get; set; }
    }
}
