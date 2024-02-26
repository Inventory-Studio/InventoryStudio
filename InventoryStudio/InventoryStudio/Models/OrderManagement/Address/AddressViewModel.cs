using ISLibrary;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace InventoryStudio.Models.OrderManagement.Address
{
    public class AddressViewModel
    {
        [DisplayName("Address ID")]
        public string AddressID { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Full Name")]
        public string? FullName { get; set; }

        [DisplayName("Attention")]
        public string? Attention { get; set; }

        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }

        [DisplayName("Address 1")]
        public string? Address1 { get; set; }

        [DisplayName("Address 2")]
        public string? Address2 { get; set; }

        [DisplayName("Address 3")]
        public string? Address3 { get; set; }

        [DisplayName("City")]
        public string? City { get; set; }

        [DisplayName("State")]
        public string? State { get; set; }

        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }

        [DisplayName("Country")]
        public string? Country { get; set; }

        [DisplayName("E-mail")]
        public string? Email { get; set; }

        [DisplayName("Phone")]
        public string? Phone { get; set; }

        [DisplayName("Zone")]
        public string? Zone { get; set; }

        [DisplayName("Is Invalid Address")]
        public bool IsInvalidAddress { get; set; }

        [DisplayName("Is Address Updated")]
        public bool IsAddressUpdated { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [ValidateNever]
        public List<AuditData>? AuditDataList { get; set; }
    }
}
