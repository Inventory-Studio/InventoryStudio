using Azure.Core;
using InventoryStudio.Models;
using InventoryStudio.Models.OrderManagement.Client;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryStudio.Controllers.OrderManagement
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var clients = Client.GetClients(company.Value);
            var list = new List<ClientViewModel>();
            foreach (var client in clients)
            {
                list.Add(ClientConvertViewModel(client));
            }
            return View(list);
        }

        private ClientViewModel ClientConvertViewModel(Client client)
        {
            var viewModel = new ClientViewModel();
            viewModel.ClientId = client.ClientID;
            viewModel.CompanyName = client.CompanyName;
            viewModel.FirstName = client.FirstName;
            viewModel.LastName = client.LastName;
            viewModel.EmailAddress = client.EmailAddress;
            viewModel.CreatedOn = client.CreatedOn;
            viewModel.UpdatedOn = client.UpdatedOn;
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

        public async Task<IActionResult> Details(string? id)
        {

            if (id == null)
                return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var client = new Client(company.Value, id);
            if (client == null)
                return NotFound();
            var detailViewModel = ClientConvertViewModel(client);
            return View(detailViewModel);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CompanyId,CompanyName,FirstName,LastName,EmailAddress")] CreateClientViewModel input)
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
            var companies = user.Companies;
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName", input.CompanyId);
            return View(input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var client = new Client(company.Value, id);
            if (client == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName");
            var viewModel = new EditClientViewModel();
            viewModel.ClientID = client.ClientID;
            viewModel.CompanyID = client.CompanyID;
            viewModel.CompanyName = client.CompanyName;
            viewModel.FirstName = client.FirstName;
            viewModel.LastName = client.LastName;
            viewModel.EmailAddress = client.EmailAddress;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("ClientId,CompanyId,CompanyName,FirstName,LastName,EmailAddress")] EditClientViewModel input)
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
            var companies = user.Companies;
            ViewData["CompanyId"] = new SelectList(companies, "CompanyId", "CompanyName");
            return View(input);
        }

        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var client = new Client(company.Value, id);
            var viewModel = ClientConvertViewModel(client);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (id == null) return NotFound();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            var client = new Client(company.Value, id);
            if (client == null)
                client.Delete();
            return RedirectToAction(nameof(Index));
        }
    }
}
