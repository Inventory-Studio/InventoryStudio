﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryStudio.Models.Templates
{
    public class CreateTemplateViewModel
    {

        [DisplayName("Company ID")]
        public string CompanyID { get; set; }

        [DisplayName("Template Name")]
        public string TemplateName { get; set; }

        public TemplateType Type { get; set; }

        [DisplayName("Import Type")]
        public TemplateImportType ImportType { get; set; }

        [DisplayName("File")]
        public IFormFile File { get; set; }
    }

    public enum TemplateType
    {
        Vendor,
        Customer,
        PurchaseOrder,
        SalesOrder
    }

    public enum TemplateImportType
    {
        Add,
        Update,
        [Display(Name = "Add Or Update")]
        AddOrUpdate
    }
}