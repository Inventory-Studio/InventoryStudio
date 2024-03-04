using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class CustomerTemplate
    {
        /// <summary>
        /// references Company Table CompanyID
        /// </summary>
        [Required]
        public string Company { get; set; } = null!;

        /// <summary>
        ///  references Client Table ClientID
        /// </summary>
        public string? Client { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        /// <summary>
        ///  references Address Table AddressID
        /// </summary>
        public string? DefaultBillingAddress { get; set; }

        /// <summary>
        /// references Address Table AddressID
        /// </summary>
        public string? DefaultShippingAddress { get; set; }

        public string? ExternalID { get; set; }

    }
}
