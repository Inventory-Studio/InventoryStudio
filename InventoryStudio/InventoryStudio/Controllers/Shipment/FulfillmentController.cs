using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ISLibrary;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using Newtonsoft.Json;
using ISLibrary.OrderManagement;
using System.ComponentModel.Design;

namespace InventoryStudio.Controllers
{
    public class FulfillmentController : BaseController

    { 
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public FulfillmentController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "Shipment-Fulfillment-List")]
        public async Task<IActionResult> IndexAsync()
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim != null)
            {
                //use ViewBag to control Button show/hide
                var permissions = new Dictionary<string, bool>
                {
                    ["CanCreate"] = (await _authorizationService.AuthorizeAsync(User, "Shipment-Fulfillment-Create")).Succeeded,
                    ["CanView"] = (await _authorizationService.AuthorizeAsync(User, "Shipment-Fulfillment-View")).Succeeded,
                    ["CanDelete"] = (await _authorizationService.AuthorizeAsync(User, "Shipment-Fulfillment-Delete")).Succeeded,
                };
                ViewBag.Permissions = permissions;


                return View("~/Views/Shipment/Fulfillment/Index.cshtml");
            }

            ViewBag.ErrorMessage = "Please create or Choose Comapny";


            return View("Error");
        }


        [Authorize(Policy = "Shipment-Fulfillment-Create")]
        public IActionResult Create(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var salesOrder = new SalesOrder(organizationClaim.Value, id);
            if (salesOrder == null)
                return NotFound();

            FulfillmentViewModel fulfillmentViewModel = new FulfillmentViewModel();
            fulfillmentViewModel.LocationID = salesOrder.LocationID;
            fulfillmentViewModel.SalesOrderID = salesOrder.SalesOrderID;
            fulfillmentViewModel.SalesOrderLines = salesOrder.SalesOrderLines;


            return View("~/Views/Shipment/Fulfillment/Create.cshtml", fulfillmentViewModel);
        }


        [Authorize(Policy = "Shipment-Fulfillment-Create")]
        [HttpPost]
        public IActionResult Create(FulfillmentViewModel fulfillmentViewModel)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                ModelState.AddModelError("", "Invalid organization information.");
                return View("~/Views/Shipment/Fulfillment/Create.cshtml", fulfillmentViewModel);
            }

            Fulfillment objFulfillment = new Fulfillment();
            objFulfillment.CreatedBy = Convert.ToString(_userManager.GetUserId(User));
            objFulfillment.CompanyID = organizationClaim.Value;
            objFulfillment.SalesOrderID = fulfillmentViewModel.SalesOrderID;
            objFulfillment.LocationID = fulfillmentViewModel.LocationID;
            objFulfillment.SalesOrderLines = fulfillmentViewModel.SalesOrderLines;
            try
            {
                objFulfillment.Create();
                return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
                return View("~/Views/Shipment/Fulfillment/Create.cshtml", fulfillmentViewModel);
            }
        }

        [Authorize(Policy = "Shipment-Fulfillment-View")]
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objFulfillment = new Fulfillment(organizationClaim.Value,id);

            if (objFulfillment == null)
            {
                return NotFound();
            }
            FulfillmentDetailViewModel fulfillmentDetailViewModel = new FulfillmentDetailViewModel();
            fulfillmentDetailViewModel.Fulfillment = objFulfillment;
            

            return View("~/Views/Shipment/Fulfillment/Details.cshtml", fulfillmentDetailViewModel);
        } 
        
        [Authorize(Policy = "Shipment-Fulfillment-PickUp")]
        public IActionResult PickUp(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objFulfillment = new Fulfillment(organizationClaim.Value,id);

            if (objFulfillment == null)
            {
                return NotFound();
            }
            if(objFulfillment.Status != Convert.ToString(Fulfillment.enumFulfillmentStatus.Pending))
            {
                ModelState.AddModelError("created_error", "Fulfillment Status is wrong");
                return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
            }
        

            objFulfillment.PickedUpBy = Convert.ToString(_userManager.GetUserId(User));
            objFulfillment.UpdatedBy = Convert.ToString(_userManager.GetUserId(User));
            objFulfillment.PickedUpOn = DateTime.UtcNow; 
            objFulfillment.IsPickedUp = true; 
            objFulfillment.Status = Convert.ToString(Fulfillment.enumFulfillmentStatus.PickedUp); 
            try
            {
                objFulfillment.Update();
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);
               
            }

            return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
        }

        [Authorize(Policy = "Shipment-Fulfillment-Pick")]
        public IActionResult Pick(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objFulfillment = new Fulfillment(organizationClaim.Value, id);

            if (objFulfillment == null)
            {
                return NotFound();
            }
            if (objFulfillment.Status != Convert.ToString(Fulfillment.enumFulfillmentStatus.PickedUp))
            {
                ModelState.AddModelError("created_error", "Fulfillment Status is wrong");
                return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
            }


            objFulfillment.PickedBy = Convert.ToString(_userManager.GetUserId(User));
            objFulfillment.PickedOn = DateTime.UtcNow;
            objFulfillment.IsPicked = true;
            objFulfillment.Status = Convert.ToString(Fulfillment.enumFulfillmentStatus.Picked);
            objFulfillment.UpdatedBy = Convert.ToString(_userManager.GetUserId(User));
            try
            {
                objFulfillment.Update();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);

            }

            return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
        }

        [Authorize(Policy = "Shipment-Fulfillment-Ship")]
        public IActionResult Ship(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objFulfillment = new Fulfillment(organizationClaim.Value, id);

            if (objFulfillment == null)
            {
                return NotFound();
            }
            if (objFulfillment.Status != Convert.ToString(Fulfillment.enumFulfillmentStatus.Picked))
            {
                ModelState.AddModelError("created_error", "Fulfillment Status is wrong");
                return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
            }


            objFulfillment.PackedBy = Convert.ToString(_userManager.GetUserId(User));
            objFulfillment.PackedOn = DateTime.UtcNow; 
            objFulfillment.ShippedBy = Convert.ToString(_userManager.GetUserId(User));
            objFulfillment.ShippedOn = DateTime.UtcNow;
            objFulfillment.IsPacked = true;
            objFulfillment.IsShipped = true;
            objFulfillment.Status = Convert.ToString(Fulfillment.enumFulfillmentStatus.Shipped);
            objFulfillment.UpdatedBy = Convert.ToString(_userManager.GetUserId(User));
            try
            {
                objFulfillment.Update();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);

            }

            return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
        }

        [Authorize(Policy = "Shipment-Fulfillment-CreateFulfillmentPackage")]
        public IActionResult CreateFulfillmentPackage(FulfillmentPackage fulfillmentPackage)
        {
            if (fulfillmentPackage.FulfillmentID == null)
            {
                return RedirectToAction("Index");
            }

            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objFulfillment = new Fulfillment(organizationClaim.Value, fulfillmentPackage.FulfillmentID);

            if (objFulfillment == null)
            {
                return NotFound();
            }

            FulfillmentPackage objFulfillmentPackage = new FulfillmentPackage();
            objFulfillmentPackage = fulfillmentPackage;
            try
            {
                objFulfillmentPackage.CreatedBy = Convert.ToString(_userManager.GetUserId(User));
                objFulfillmentPackage.CompanyID = organizationClaim.Value;
                objFulfillmentPackage.Create();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("created_error", ex.Message);

            }

            return RedirectToAction("Details", new { id = objFulfillment.FulfillmentID });
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<Fulfillment> dataSource = new List<Fulfillment>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    FulfillmentFilter FulfillmentFilter = new FulfillmentFilter();

                    dataSource = Fulfillment.GetFulfillments(
                        company.Value,
                        FulfillmentFilter,
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
