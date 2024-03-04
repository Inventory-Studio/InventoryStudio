using System.ComponentModel.DataAnnotations;

namespace ISLibrary.Template
{
    public class VendorTemplate
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

        public string? VendorNumber { get; set; }

        public string? ExternalID { get; set; }

        public string? CompanyName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }
    }
}
