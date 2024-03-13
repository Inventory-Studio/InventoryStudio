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
    public class ItemController : BaseController

    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ItemController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "Inventory-Item-List")]
        public async Task<IActionResult> IndexAsync()
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                var items = Item.GetItems(organizationClaim.Value);

                //use ViewBag to control Button show/hide
                var permissions = new Dictionary<string, bool>
                {
                    ["CanCreate"] = (await _authorizationService.AuthorizeAsync(User, "Inventory-Item-Create"))
                        .Succeeded,
                    ["CanEdit"] = (await _authorizationService.AuthorizeAsync(User, "Inventory-Item-Edit")).Succeeded,
                    ["CanDelete"] = (await _authorizationService.AuthorizeAsync(User, "Inventory-Item-Delete"))
                        .Succeeded,
                };
                ViewBag.Permissions = permissions;


                return View("~/Views/Inventory/Item/Index.cshtml", items);
            }

            ViewBag.ErrorMessage = "Please create or Choose Comapny";


            return View("Error");
        }


        [Authorize(Policy = "Inventory-Item-Create")]
        public IActionResult Create()
        {
            var itemViewModel = new ItemViewModel
            {
                Item = new Item
                {
                    ItemBarcodes = new List<ItemBarcode>()
                }
            };
            return View("~/Views/Inventory/Item/Create.cshtml", itemViewModel);
        }

        [Authorize(Policy = "Inventory-Item-Create")]
        [HttpPost]
        public IActionResult Create([FromForm] ItemViewModel itemViewModel)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                ModelState.AddModelError("", "Invalid organization information.");
                return Json(new { status = "error", message = "Invalid organization information." });
            }

            itemViewModel.Item.CreatedBy = Convert.ToString(_userManager.GetUserId(User));
            itemViewModel.Item.CompanyID = organizationClaim.Value;
            if (itemViewModel?.Item != null && itemViewModel?.Item?.ItemBarcodes != null)
            {
                var hasDefaultBarcode = itemViewModel.Item.ItemBarcodes?.Select(x =>
                    x.Barcode.Trim().ToLower().Equals(itemViewModel.Item.ItemNumber.Trim().ToLower())).First() ?? false;
                if (!hasDefaultBarcode)
                {
                    itemViewModel?.Item?.ItemBarcodes?.Add(new ItemBarcode
                        { Barcode = itemViewModel.Item.ItemNumber, Type = "Item Number" });
                }
            }
            else
            {
                if (itemViewModel != null && itemViewModel.Item != null)
                {
                        itemViewModel.Item.ItemBarcodes = new List<ItemBarcode>
                        {
                            new() { Barcode = itemViewModel.Item.ItemNumber, Type = "Item Number" }
                        };
                }
            }

            try

            {
                ItemParent.CreateItem(itemViewModel.Item, itemViewModel.ItemAttributes, itemViewModel.ItemMatrices);
                return Json(new
                    {
                        status = "success", redirect = Url.Action("Edit", "Item", new
                        {
                            id = itemViewModel.Item.ItemID
                        })
                    }
                );
            }
            catch
                (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
                return Json(new { status = "error", message = "Error creating item. " + ex.Message }
                );
            }
        }

        [Authorize(Policy = "Inventory-Item-Edit")]
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var itemViewModel = new ItemViewModel();

            if (TempData["FormData"] != null)
            {
                string formData = TempData["FormData"] as string;
                itemViewModel = JsonConvert.DeserializeObject<ItemViewModel>(formData);

                if (TempData["ErrorMessage"] != null)
                {
                    ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
                }
            }
            else
            {
                var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
                var item = new Item(organizationClaim.Value, id);

                if (item == null)
                {
                    return NotFound();
                }

                var itemParent = new ItemParent(organizationClaim.Value, item.ItemParentID);
                var auditDataList = AuditData.GetAuditDatas("Item", id);

                itemViewModel.Item = item;
                itemViewModel.ItemParent = itemParent;
                itemViewModel.ItemAttributes = itemParent.ItemAttributes;
                itemViewModel.ItemMatrices = itemParent.ItemMatrices;
                itemViewModel.AuditDataList = auditDataList;
            }


            return View("~/Views/Inventory/Item/Edit.cshtml", itemViewModel);
        }

        [Authorize(Policy = "Inventory-Item-Edit")]
        [HttpPost]
        public IActionResult Edit([FromForm] ItemViewModel itemViewModel)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");

            var item = new Item(organizationClaim.Value, itemViewModel.Item.ItemID);

            itemViewModel.Item.CompanyID = item.CompanyID;
            itemViewModel.Item.ItemParentID = item.ItemParentID;
            itemViewModel.Item.UpdatedBy = Convert.ToString(_userManager.GetUserId(User));
            try
            {
                ItemParent.UpdateItem(itemViewModel.Item, itemViewModel.ItemAttributes, itemViewModel.ItemMatrices);
                return Json(new
                    {
                        status = "success", redirect = Url.Action("Edit", "Item", new
                        {
                            id = itemViewModel.Item.ItemID
                        })
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData["FormData"] = JsonConvert.SerializeObject(itemViewModel);
                return Json(new
                    {
                        status = "error", message = "An error occured. " + ex.Message
                    }
                );
            }
        }


        public IActionResult Details(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var itemDetailsViewModel = new ItemDetailsViewModel();

            if (TempData["FormData"] != null)
            {
                string formData = TempData["FormData"] as string;
                itemDetailsViewModel = JsonConvert.DeserializeObject<ItemDetailsViewModel>(formData);

                if (TempData["ErrorMessage"] != null)
                {
                    ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
                }
            }
            else
            {
                var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
                var item = new Item(organizationClaim.Value, id);

                if (item == null)
                {
                    return NotFound();
                }

                var itemParent = new ItemParent(organizationClaim.Value, item.ItemParentID);
                var auditDataList = AuditData.GetAuditDatas("Item", id);

                itemDetailsViewModel.IsDetails = true;
                itemDetailsViewModel.Item = item;
                itemDetailsViewModel.ItemParent = itemParent;
                itemDetailsViewModel.ItemAttributes = itemParent.ItemAttributes;
                itemDetailsViewModel.ItemMatrices = itemParent.ItemMatrices;
                itemDetailsViewModel.AuditDataList = auditDataList;
                itemDetailsViewModel.ProductStatus = "Active";
                itemDetailsViewModel.OnHand = 100;
                itemDetailsViewModel.Available = 98;
                itemDetailsViewModel.ProductSku = item.ItemNumber;
                // itemDetailsViewModel.ProductImage =
                //     "https://as1.ftcdn.net/v2/jpg/01/81/20/94/1000_F_181209420_P2Pa9vacolr2uIOwSJdCq4w5ydtPCAsS.jpg";
                itemDetailsViewModel.ShipMonkImage =
                    "https://as1.ftcdn.net/v2/jpg/01/81/20/94/1000_F_181209420_P2Pa9vacolr2uIOwSJdCq4w5ydtPCAsS.jpg";
            }

            return View("~/Views/Inventory/Item/Details.cshtml", itemDetailsViewModel);
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
                    ItemFilter itemFilter = new ItemFilter();

                    dataSource = Item.GetItems(
                        company.Value,
                        itemFilter,
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

            var data = dataSource.Select(item => new
            {
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

            var data = dataSource.Select(item => new
            {
                ItemName = item.ItemNumber + '-' + item.ItemName,
                ItemID = item.ItemID
            });

            return Json(new { dataSource = data });
        }
    }
}