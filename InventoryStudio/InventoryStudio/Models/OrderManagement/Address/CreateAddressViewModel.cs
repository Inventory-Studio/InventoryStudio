namespace InventoryStudio.Models.OrderManagement.Address
{
    public class CreateAddressViewModel
    {

        public string CompanyID { get; set; }

        public string? FullName { get; set; }

        public string? Attention { get; set; }

        public string? CompanyName { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? Address3 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public string? CountryID { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Zone { get; set; }

        public bool IsInvalidAddress { get; set; }

        public bool IsAddressUpdated { get; set; }


    }
}
