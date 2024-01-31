﻿using InventoryStudio.Models.OrderManagement.Client;
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

        public IActionResult Insert([FromBody] CRUDModel<Client> value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<Client> value)
        {
            return Json(value.Value ?? new Client());
        }


        public IActionResult Remove([FromBody] CRUDModel<Client> value)
        {
            if (value.Key != null)
            {
                DeleteConfirmed(value.Value.ClientID);
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<Client> dataSource = new List<Client>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    ClientFilter clientFilter = new();
                    dataSource = Client.GetClients(
                        company.ToString(),
                        clientFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    );
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