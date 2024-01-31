namespace InventoryStudio.Models.OrderManagement.Address
{
    public class AddressViewModel
    {
        public string AddressID { get; set; }

        public string Company { get; set; }

        public string? FullName { get; set; }

        public string? Attention { get; set; }

        public string? CompanyName { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? Address3 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Zone { get; set; }

        public bool IsInvalidAddress { get; set; }

        public bool IsAddressUpdated { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
