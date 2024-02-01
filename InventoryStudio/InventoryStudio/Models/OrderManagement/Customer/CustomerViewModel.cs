namespace InventoryStudio.Models.OrderManagement.Customer
{
    public class CustomerViewModel
    {
        public string CustomerID { get; set; }

        public string Company { get; set; }

        public string Client { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? ExternalID { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
