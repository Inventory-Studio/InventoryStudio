using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLibrary.OrderManagement
{
    public class Customer
    {
        public string CustomerID { get; set; } 

        public string CompanyID { get; set; }

        public string? ClientID { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? ExternalID { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdtedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
