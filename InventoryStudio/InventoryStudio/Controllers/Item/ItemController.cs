using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ISLibrary;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers
{
    public class ItemController : Controller

    { 
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ItemController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "Item-Item-List")]
        public async Task<IActionResult> IndexAsync()
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                var items = Item.GetItems(organizationClaim.Value);

                //use ViewBag to control Button show/hide
                var permissions = new Dictionary<string, bool>
                {
                    ["CanCreate"] = (await _authorizationService.AuthorizeAsync(User, "Item-Item-Create")).Succeeded,
                    ["CanEdit"] = (await _authorizationService.AuthorizeAsync(User, "Item-Item-Edit")).Succeeded,
                    ["CanDelete"] = (await _authorizationService.AuthorizeAsync(User, "Item-Item-Delete")).Succeeded,
                };
                ViewBag.Permissions = permissions;


                return View("~/Views/Item/Item/Index.cshtml", items);
            }

            ViewBag.ErrorMessage = "Please create or Choose Comapny";


            return View("Error");
        }


        [Authorize(Policy = "Item-Item-Create")]
        public IActionResult Create()
        {
            return View("~/Views/Item/Item/Create.cshtml");
        }

        [Authorize(Policy = "Item-Item-Create")]
        [HttpPost]
        public IActionResult Create(Item item)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                ModelState.AddModelError("", "Invalid organization information.");
                return View("~/Views/Item/Item/Create.cshtml", item);
            }

            item.CreatedBy = Convert.ToString(_userManager.GetUserId(User));
            item.CompanyID = organizationClaim.Value;

            try
            {
                ItemParent.CreateItem(item);
                return View("~/Views/Item/Item/Create.cshtml", item);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
                return View("~/Views/Item/Item/Create.cshtml", item);
            }
        }


        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<Item> dataSource = new List<Item>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    

                    dataSource = Item.GetItems(
                        company.Value,
                        null,
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


        public IActionResult GetCompoentChildItems()
        {
            IEnumerable<Item> dataSource = new List<Item>().AsEnumerable();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company != null)
            {
                dataSource = Item.GetItems(company.Value).AsEnumerable();
            }

            var data = dataSource.Select(item => new {
                ItemName = item.ItemName,
                ItemID = item.ItemID
            });

            return Json(new { dataSource = data });
        }


        public IActionResult GetKitChildItems()
        {
            IEnumerable<Item> dataSource = new List<Item>().AsEnumerable();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company != null)
            {
                dataSource = Item.GetItems(company.Value).AsEnumerable();
            }

            var data = dataSource.Select(item => new {
                ItemName = item.ItemName,
                ItemID = item.ItemID
            });

            return Json(new { dataSource = data });
        }
    }
}
