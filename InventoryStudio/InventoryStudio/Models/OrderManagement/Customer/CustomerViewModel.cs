using System.ComponentModel;

namespace InventoryStudio.Models.OrderManagement.Customer
{
    public class CustomerViewModel
    {
        [DisplayName("Customer ID")]
        public string CustomerID { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Client")]
        public string Client { get; set; }

        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }

        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        [DisplayName("E-mail")]
        public string? EmailAddress { get; set; }

        [DisplayName("External ID")]
        public string? ExternalID { get; set; }

        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
    }
}
