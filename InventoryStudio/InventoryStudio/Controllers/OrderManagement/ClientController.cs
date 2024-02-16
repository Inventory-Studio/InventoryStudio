using InventoryStudio.Models.OrderManagement.Client;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.Json;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class ClientController : Controller
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientController(IHttpContextAccessor httpContextAccessor)
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
            return View("~/Views/OrderManagement/Client/Index.cshtml");
        }

        private ClientViewModel ClientConvertViewModel(Client client)
        {
            ClientViewModel viewModel = new()
            {
                ClientId = client.ClientID,
                CompanyName = client.CompanyName,
                FirstName = client.FirstName,
                LastName = client.LastName,
                EmailAddress = client.EmailAddress,
                CreatedOn = client.CreatedOn,
                UpdatedOn = client.UpdatedOn
            };
            if (!string.IsNullOrEmpty(client.CreatedBy))
            {
                var createdUser = new AspNetUsers(client.CreatedBy);
                if (createdUser != null)
                    viewModel.CreatedBy = createdUser.UserName;
            }

            if (!string.IsNullOrEmpty(client.UpdatedBy))
            {
                var updatedUser = new AspNetUsers(client.UpdatedBy);
                if (updatedUser != null)
                    viewModel.UpdatedBy = updatedUser.UserName;
            }

            return viewModel;
        }

        public IActionResult Details(string? id)
        {
            if (id == null)
                return NotFound();
            if (string.IsNullOrEmpty(CompanyID))
                return NotFound();

            var client = new Client(CompanyID, id);
            if (client == null)
                return NotFound();

            var detailViewModel = ClientConvertViewModel(client);
            return View("~/Views/OrderManagement/Client/Details.cshtml", detailViewModel);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View("~/Views/OrderManagement/Client/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("CompanyID,CompanyName,FirstName,LastName,EmailAddress")]
            CreateClientViewModel input)
        {
            if (ModelState.IsValid)
            {
                var client = new Client();
                client.CompanyID = input.CompanyID;
                client.CompanyName = input.CompanyName;
                client.FirstName = input.FirstName;
                client.LastName = input.LastName;
                client.EmailAddress = input.EmailAddress;
                client.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                client.Create();
                return RedirectToAction(nameof(Index));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies ?? new List<Company>();
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName", input.CompanyID);
            return View("~/Views/OrderManagement/Client/Create.cshtml");
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            if (string.IsNullOrEmpty(CompanyID))
                return NotFound();
            var client = new Client(CompanyID, id);
            if (client == null)
                return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            EditClientViewModel viewModel = new()
            {
                ClientID = client.ClientID,
                CompanyID = client.CompanyID,
                CompanyName = client.CompanyName,
                FirstName = client.FirstName,
                LastName = client.LastName,
                EmailAddress = client.EmailAddress
            };
            return View("~/Views/OrderManagement/Client/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id,
            [Bind("ClientID,CompanyID,CompanyName,FirstName,LastName,EmailAddress")]
            EditClientViewModel input)
        {
            if (id != input.ClientID)
                return NotFound();

            if (ModelState.IsValid)
            {
                var client = new Client(id);
                client.ClientID = input.ClientID;
                client.CompanyID = input.CompanyID;
                client.CompanyName = input.CompanyName;
                client.FirstName = input.FirstName;
                client.LastName = input.LastName;
                client.EmailAddress = input.EmailAddress;
                client.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                client.Update();
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies ?? new List<Company>();
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View("~/Views/OrderManagement/Client/Edit.cshtml");
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            if (string.IsNullOrEmpty(CompanyID))
                return NotFound();
            var client = new Client(CompanyID, id);
            var viewModel = ClientConvertViewModel(client);
            return View("~/Views/OrderManagement/Client/Delete.cshtml", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            if (string.IsNullOrEmpty(CompanyID))
                return NotFound();
            var client = new Client(CompanyID, id);
            if (client != null)
                client.Delete();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Insert([FromBody] CRUDModel<ClientViewModel> value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<ClientViewModel> value)
        {
            return Json(value.Value ?? new ClientViewModel());
        }


        public IActionResult Remove([FromBody] CRUDModel<ClientViewModel> value)
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
            IEnumerable<ClientViewModel> dataSource = new List<ClientViewModel>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                if (!string.IsNullOrEmpty(CompanyID))
                {
                    ClientFilter clientFilter = new();
                    dataSource = Client.GetClients(
                        CompanyID,
                        clientFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    ).Select(ClientConvertViewModel);
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