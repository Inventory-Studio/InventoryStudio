using InventoryStudio.Models.OrderManagement.Customer;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.Json;
using InventoryStudio.Models.OrderManagement.Client;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class CustomerController : Controller
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                Claim? company = user.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                    CompanyID = company.Value;
            }
        }

        public IActionResult Index()
        {
            var customers = Customer.GetCustomers(CompanyID);
            var list = new List<CustomerViewModel>();
            foreach (var customer in customers)
            {
                list.Add(EntityConvertViewModel(customer));
            }

            return View("~/Views/OrderManagement/Customer/Index.cshtml", list);
        }

        private CustomerViewModel EntityConvertViewModel(Customer customer)
        {
            var viewModel = new CustomerViewModel();
            viewModel.CustomerID = customer.CustomerID;
            if (!string.IsNullOrEmpty(customer.CompanyID))
            {
                var company = new Company(customer.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }

            if (!string.IsNullOrEmpty(customer.ClientID))
            {
                var client = new Client(CompanyID, customer.ClientID);
                if (client != null)
                    viewModel.Client = client.EmailAddress;
            }

            viewModel.CompanyName = customer.CompanyName;
            viewModel.FirstName = customer.FirstName;
            viewModel.LastName = customer.LastName;
            viewModel.EmailAddress = customer.EmailAddress;
            viewModel.ExternalID = customer.ExternalID;
            if (!string.IsNullOrEmpty(customer.CreatedBy))
            {
                var user = new AspNetUsers(customer.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }

            viewModel.CreatedOn = customer.CreatedOn;
            if (!string.IsNullOrEmpty(customer.UpdatedBy))
            {
                var user = new AspNetUsers(customer.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }

            viewModel.UpdatedOn = customer.UpdatedOn;
            return viewModel;
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var location = new Customer(CompanyID, id);
            if (location == null)
                return NotFound();
            var detailViewModel = EntityConvertViewModel(location);
            return View("~/Views/OrderManagement/Customer/Details.cshtml", detailViewModel);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var clients = Client.GetClients(CompanyID);
            ViewData["ClientID"] = new SelectList(clients, "ClientID", "EmailAddress");
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View("~/Views/OrderManagement/Customer/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("CompanyID,ClientID,CompanyName,FirstName,LastName,EmailAddress,ExternalID")]
            CreateCustomerViewModel input)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer();
                customer.CompanyID = input.CompanyID;
                customer.ClientID = input.ClientID;
                customer.CompanyName = input.CompanyName;
                customer.FirstName = input.FirstName;
                customer.LastName = input.LastName;
                customer.EmailAddress = input.EmailAddress;
                customer.ExternalID = input.ExternalID;
                customer.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                customer.Create();
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var clients = Client.GetClients(CompanyID);
            ViewData["ClientID"] = new SelectList(clients, "ClientID", "EmailAddress", input.ClientID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            return View("~/Views/OrderManagement/Customer/Create.cshtml", input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var customer = new Customer(CompanyID, id);
            if (customer == null)
                return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var clients = Client.GetClients(CompanyID);
            ViewData["ClientID"] = new SelectList(clients, "ClientID", "EmailAddress", customer.ClientID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", customer.CompanyID);
            var viewModel = new EditCustomerViewModel();
            viewModel.CustomerID = customer.CustomerID;
            viewModel.CompanyID = customer.CompanyID;
            viewModel.ClientID = customer.ClientID;
            viewModel.CompanyName = customer.CompanyName;
            viewModel.FirstName = customer.FirstName;
            viewModel.LastName = customer.LastName;
            viewModel.EmailAddress = customer.EmailAddress;
            viewModel.ExternalID = customer.ExternalID;
            return View("~/Views/OrderManagement/Customer/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id,
            [Bind("CustomerID,CompanyID,ClientID,CompanyName,FirstName,LastName,EmailAddress,ExternalID")]
            EditCustomerViewModel input)
        {
            if (id != input.CustomerID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var customer = new Customer(CompanyID, input.CustomerID);
                if (customer == null)
                    return NotFound();
                customer.CompanyID = input.CompanyID;
                customer.ClientID = input.ClientID;
                customer.CompanyName = input.CompanyName;
                customer.FirstName = input.FirstName;
                customer.LastName = input.LastName;
                customer.EmailAddress = input.EmailAddress;
                customer.ExternalID = input.ExternalID;
                customer.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                customer.Update();
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            var clients = Client.GetClients(CompanyID);
            ViewData["ClientID"] = new SelectList(clients, "ClientID", "EmailAddress", input.ClientID);
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            return View("~/Views/OrderManagement/Customer/Edit.cshtml", input);
        }

        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();
            var customer = new Customer(CompanyID, id);
            if (customer == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(customer);
            return View("~/Views/OrderManagement/Customer/Delete.cshtml", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var customer = new Customer(CompanyID, id);
            if (customer != null)
                customer.Delete();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Insert([FromBody] CRUDModel value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<CustomerViewModel> value)
        {
            return Json(value.Value ?? new CustomerViewModel());
        }


        public IActionResult Remove([FromBody] CRUDModel<CustomerViewModel> value)
        {
            if (value.Key != null)
            {
                var id = value.Key.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    DeleteConfirmed(id);
                }
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<CustomerViewModel> dataSource = new List<CustomerViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                if (!string.IsNullOrEmpty(CompanyID))
                {
                    CustomerFilter customerFilter = new();
                    dataSource = Customer.GetCustomers(
                        CompanyID,
                        customerFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    ).Select(EntityConvertViewModel);
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