using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ISLibrary;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using Newtonsoft.Json;

namespace InventoryStudio.Controllers
{
    public class ItemUnitController : BaseController

    { 
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ItemUnitController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "Inventory-ItemUnit-List")]
        public async Task<IActionResult> IndexAsync()
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                //use ViewBag to control Button show/hide
                var permissions = new Dictionary<string, bool>
                {
                    ["CanCreate"] = (await _authorizationService.AuthorizeAsync(User, "Inventory-ItemUnit-Create")).Succeeded,
                    ["CanView"] = (await _authorizationService.AuthorizeAsync(User, "Inventory-ItemUnit-View")).Succeeded,
                    ["CanDelete"] = (await _authorizationService.AuthorizeAsync(User, "Item-ItemUnit-Delete")).Succeeded,
                };
                ViewBag.Permissions = permissions;


                return View("~/Views/Inventory/ItemUnit/Index.cshtml");
            }

            ViewBag.ErrorMessage = "Please create or Choose Comapny";


            return View("Error");
        }


        [Authorize(Policy = "Inventory-ItemUnit-Create")]
        public IActionResult Create()
        {
            return View("~/Views/Inventory/ItemUnit/Create.cshtml");
        }

        [Authorize(Policy = "Inventory-ItemUnit-Create")]
        [HttpPost]
        public IActionResult Create(ItemUnitViewModel itemUnitViewModel)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                ModelState.AddModelError("", "Invalid organization information.");
                return View("~/Views/Inventory/ItemUnit/Create.cshtml", itemUnitViewModel);
            }

            ItemUnitType objItemUnitType = new ItemUnitType();
            objItemUnitType.CreatedBy = Convert.ToString(_userManager.GetUserId(User));
            objItemUnitType.CompanyID = organizationClaim.Value;
            objItemUnitType.Name = itemUnitViewModel.Name;
            objItemUnitType.ItemUnits = itemUnitViewModel.ItemUnits;
            try
            {
                objItemUnitType.Create();
                return RedirectToAction("Details", new { id = objItemUnitType.ItemUnitTypeID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
                return View("~/Views/Inventory/ItemUnit/Create.cshtml", itemUnitViewModel);
            }
        }            


        public IActionResult Details(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objItemUnitType = new ItemUnitType(id);

            if (objItemUnitType == null)
            {
                return NotFound();
            }

            if (objItemUnitType.CompanyID != organizationClaim.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to change other company's data.";
                return RedirectToAction("Index");
            }

            var itemUnitViewModel = new ItemUnitViewModel();

            itemUnitViewModel.Name = objItemUnitType.Name;
            itemUnitViewModel.ItemUnits = objItemUnitType.ItemUnits;

            return View("~/Views/Inventory/ItemUnit/Details.cshtml", itemUnitViewModel);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<ItemUnitType> dataSource = new List<ItemUnitType>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    ItemUnitTypeFilter ItemUnitTypeFilter = new ItemUnitTypeFilter();

                    dataSource = ItemUnitType.GetItemUnitTypes(
                        company.Value,
                        ItemUnitTypeFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    ).AsEnumerable();
                }
            }

            if (dm.Search != null && dm.Search.Count > 0)
            {
                dataSource = operation.PerformSearching(dataSource, dm.Search); //Search
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                dataSource = operation.PerformSorting(dataSource, dm.Sorted);
            }

            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                if (dm.Where != null && dm.Where.Any()) //Filtering
                {
                    foreach (WhereFilter whereFilter in dm.Where)
                    {
                        if (whereFilter.IsComplex)
                        {
                            foreach (WhereFilter whereFilterPredicate in whereFilter.predicates)
                            {
                                dataSource = operation.PerformFiltering(
                                    dataSource,
                                    dm.Where,
                                    whereFilterPredicate.Operator
                                );
                            }
                        }
                        else
                        {
                            dataSource = operation.PerformFiltering(
                                dataSource,
                                dm.Where,
                                dm.Where.First().Operator
                            );
                        }
                    }
                }
            }

            if (dm.Skip != 0)
            {
                dataSource = operation.PerformSkip(dataSource, dm.Skip); //Paging
            }

            if (dm.Take != 0)
            {
                dataSource = operation.PerformTake(dataSource, dm.Take);
            }

            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = null };
            return dm.RequiresCounts
                ? Json(new { result = dataSource, count = totalRecord }, jsonSerializerOptions)
                : Json(dataSource, jsonSerializerOptions);
        }


     
    }
}
