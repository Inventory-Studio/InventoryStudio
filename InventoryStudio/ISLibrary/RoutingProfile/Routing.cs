using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary
{
    public class Routing
    {
        public enum enumFulfillmentMethod
        {
            [Description("Shortest Distance")]
            shortest_distance,
            [Description("Lowest Shipping Rate")]
            lowest_shipping_rate,
            [Description("Order of Location")]
            order_of_location
        }

        public enum enumFulfillmentStrategy
        {
            [Description("Allow Partial - Single Location")]
            allow_paratial_single_location,
            [Description("Allow Partial - Multi Location")]
            allow_partila_multi_location,
            [Description("Ship Complete - Single Location")]
            ship_complete_single_location,
            [Description("Ship Complete - Multi Location")]
            ship_complete_multi_location
        }

        public enum enumAllocationStrategy
        {
            [Description("Commit On Create")]
            commit_on_create,
            [Description("Commit Before Approval")]
            commit_before_approval,
            [Description("Commit On Approval")]
            commit_on_approval
        }
    }
}
