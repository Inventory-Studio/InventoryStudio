﻿using System.ComponentModel;

namespace InventoryStudio.Models.OrderManagement.Customer
{
    public class CreateCustomerViewModel
    {

        [DisplayName("Client ID")]
        public string? ClientID { get; set; }

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

    }
}
