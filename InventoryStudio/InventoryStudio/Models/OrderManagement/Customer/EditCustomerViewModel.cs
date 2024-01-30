namespace InventoryStudio.Models.OrderManagement.Customer
{
    public class EditCustomerViewModel
    {
        public long CustomerId { get; set; }

        public long CompanyId { get; set; }

        public long? ClientId { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? ExternalId { get; set; }

    }
}
