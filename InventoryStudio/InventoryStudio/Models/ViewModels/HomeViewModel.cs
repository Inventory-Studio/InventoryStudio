using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using InventoryStudio.Controllers;

namespace InventoryStudio.Models.ViewModels
{
    public class HomeViewModel: MainLayoutViewModel
    {
        public string? Name { get; set; }
        public string? Tagline { get; set; }
        public string? Subtitle { get; set; }
        public List<ExpandoObject>? List { get; set; }

    }
}