using InventoryStudio.File;
using InventoryStudio.Models.Templates;
using ISLibrary;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryStudio.Controllers
{
    public class TemplatesController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TemplatesController(IHttpContextAccessor httpContextAccessor)
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
            var templates = ImportTemplate.GetImportTemplates(CompanyID);
            var list = new List<TemplateViewModel>();
            foreach (var template in templates)
            {
                list.Add(EntityConvertViewModel(template));
            }
            return View(list);
        }

        private TemplateViewModel EntityConvertViewModel(ImportTemplate template)
        {
            var viewModel = new TemplateViewModel();
            viewModel.ImportTemplateID = template.ImportTemplateID;
            if (!string.IsNullOrEmpty(template.CompanyID))
            {
                var company = new Company(template.CompanyID);
                if (company != null)
                    viewModel.Company = company.CompanyName;
            }
            viewModel.TemplateName = template.TemplateName;
            viewModel.Type = template.Type;
            viewModel.ImportType = template.ImportType;
            if (!string.IsNullOrEmpty(template.UpdatedBy))
            {
                var user = new AspNetUsers(template.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }
            viewModel.UpdatedOn = template.UpdatedOn;
            if (!string.IsNullOrEmpty(template.CreatedBy))
            {
                var user = new AspNetUsers(template.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }
            viewModel.CreatedOn = template.CreatedOn;
            return viewModel;
        }


        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var template = new ImportTemplate(CompanyID, id);
            if (template == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(template);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImportTemplateID,CompanyID,TemplateName,Type,ImportType,File")] CreateTemplateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var template = new ImportTemplate();
                template.CompanyID = input.CompanyID;
                template.TemplateName = input.TemplateName;
                template.Type = input.Type.ToString();
                template.ImportType = input.ImportType.ToString();
                template.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                template.Create();

                var fileType = Path.GetExtension(input.File.FileName);
                var _fileHandler = FileHandlerFactory.CreateFileHandler<Customer>(fileType);
                var headers = await _fileHandler.GetHeader(input.File);
                var mappings = await _fileHandler.MapHeadersToEntityProperties(headers);
                foreach (var mapping in mappings)
                {
                    var templateField = new ImportTemplateField();
                    templateField.ImportTemplateID = template.ImportTemplateID;
                    templateField.CompanyID = input.CompanyID;
                    templateField.SourceField = mapping.Key;
                    templateField.DestinationTable = nameof(Customer);
                    templateField.DestinationField = mapping.Value;
                    templateField.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    templateField.CreatedOn = DateTime.Now;
                    templateField.Create();
                }
                return RedirectToAction(nameof(Index));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            return View(input);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var template = new ImportTemplate(CompanyID, id);
            if (template == null)
                return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName");
            var viewModel = new EditTemplateViewModel();
            viewModel.ImportTemplateID = template.ImportTemplateID;
            viewModel.CompanyID = template.CompanyID;
            viewModel.TemplateName = template.TemplateName;
            viewModel.Type = template.Type;
            viewModel.ImportType = template.ImportType;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("ImportTemplateID,CompanyID,TemplateName,Type,ImportType")] EditTemplateViewModel input)
        {
            if (id != input.ImportTemplateID)
                return NotFound();
            if (ModelState.IsValid)
            {
                var template = new ImportTemplate(CompanyID, id);
                if (template == null)
                    return NotFound();
                template.CompanyID = input.CompanyID;
                template.TemplateName = template.TemplateName;
                template.Type = template.Type;
                template.ImportType = template.ImportType;
                template.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                template.Update();
                return RedirectToAction(nameof(Index));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new AspNetUsers(userId);
            var companies = user.Companies;
            ViewData["CompanyID"] = new SelectList(companies, "CompanyID", "CompanyName", input.CompanyID);
            return View(input);
        }

        public IActionResult Delete(string? id)
        {
            if (id == null)
                return NotFound();
            var template = new ImportTemplate(CompanyID, id);
            if (template == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(template);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var template = new ImportTemplate(CompanyID, id);
            if (template != null)
                template.Delete();
            return RedirectToAction(nameof(Index));
        }


    }
}
