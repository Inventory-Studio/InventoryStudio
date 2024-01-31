using InventoryStudio.Models.OrderManagement.Client;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company == null)
            {
                return NotFound();
            }

            var clients = Client.GetClients(company.Value);
            var list = clients.Select(ClientConvertViewModel).ToList();
            return View("~/Views/OrderManagement/Client/Index.cshtml", list);
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
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company == null)
            {
                return NotFound();
            }

            var client = new Client(company.Value, id);
            if (client == null)
            {
                return NotFound();
            }

            var detailViewModel = ClientConvertViewModel(client);
            return View("~/Views/OrderManagement/Client/Details.cshtml", detailViewModel);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            CreateClientViewModel createClientViewModel = new();
            return View("~/Views/OrderManagement/Client/Create.cshtml", createClientViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("CompanyId,CompanyName,FirstName,LastName,EmailAddress")]
            CreateClientViewModel input)
        {
            if (ModelState.IsValid)
            {
                var client = new Client();
                client.CompanyID = input.CompanyId;
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
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName", input.CompanyId);
            return View("~/Views/OrderManagement/Client/Create.cshtml");
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company == null)
            {
                return NotFound();
            }

            var client = new Client(company.Value, id);
            if (client == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName");
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
            [Bind("ClientId,CompanyId,CompanyName,FirstName,LastName,EmailAddress")]
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
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName");
            return View("~/Views/OrderManagement/Client/Edit.cshtml");
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company == null)
            {
                return NotFound();
            }

            var client = new Client(company.Value, id);
            var viewModel = ClientConvertViewModel(client);
            return View("~/Views/OrderManagement/Client/Delete.cshtml", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company == null)
            {
                return NotFound();
            }

            var client = new Client(company.Value, id);
            if (client != null)
                client.Delete();
            return RedirectToAction(nameof(Index));
        }
    }
}