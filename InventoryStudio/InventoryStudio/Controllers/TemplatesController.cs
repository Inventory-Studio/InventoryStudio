using InventoryStudio.File;
using InventoryStudio.Importer;
using InventoryStudio.Models;
using InventoryStudio.Models.Templates;
using InventoryStudio.Services.File;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using ISLibrary.OrderManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Claims;
using System.Text;

namespace InventoryStudio.Controllers
{
    public class TemplatesController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CustomerImporter _customerImporter;
        private readonly VendorImporter _vendorImporter;
        private static Dictionary<string, int> importProgress = new Dictionary<string, int>();

        public TemplatesController(IHttpContextAccessor httpContextAccessor, CustomerImporter customerImporter, VendorImporter vendorImporter)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                Claim? company = user.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                    CompanyID = company.Value;
            }
            _customerImporter = customerImporter;
            _vendorImporter = vendorImporter;
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

        private TemplateFieldViewModel EntityConvertViewModel(ImportTemplateField templateField)
        {
            var viewModel = new TemplateFieldViewModel();
            viewModel.ImportTemplateFieldID = templateField.ImportTemplateFieldID;
            viewModel.SourceField = templateField.SourceField;
            viewModel.DestinationTable = templateField.DestinationTable;
            viewModel.DestinationField = templateField.DestinationField;
            if (!string.IsNullOrEmpty(templateField.UpdatedBy))
            {
                var user = new AspNetUsers(templateField.UpdatedBy);
                if (user != null)
                    viewModel.UpdatedBy = user.UserName;
            }
            viewModel.UpdatedOn = templateField.UpdatedOn;
            if (!string.IsNullOrEmpty(templateField.CreatedBy))
            {
                var user = new AspNetUsers(templateField.CreatedBy);
                if (user != null)
                    viewModel.CreatedBy = user.UserName;
            }
            viewModel.CreatedOn = templateField.CreatedOn;
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
            ImportTemplateFieldFilter fieldFilter = new ImportTemplateFieldFilter();
            fieldFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            fieldFilter.ImportTemplateID.SearchString = id;
            var templateFields = ImportTemplateField.GetImportTemplateFields(CompanyID, fieldFilter);
            var templateFieldViewModels = new List<TemplateFieldViewModel>();
            foreach (var templateField in templateFields)
            {
                templateFieldViewModels.Add(EntityConvertViewModel(templateField));
            }
            viewModel.TemplateFields = templateFieldViewModels;
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

        private string GetFullNamespace(string typeName)
        {
            return $"ISLibrary.Template.{typeName}Template";
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
                var fullNamespace = GetFullNamespace(input.Type.ToString());
                var _fileHandler = FileHandlerFactory.CreateFileHandlerInstance(fullNamespace, fileType);
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
            var editTemplateViewModel = new EditTemplateViewModel();
            editTemplateViewModel.ImportTemplateID = template.ImportTemplateID;
            editTemplateViewModel.CompanyID = template.CompanyID;
            editTemplateViewModel.TemplateName = template.TemplateName;
            editTemplateViewModel.Type = template.Type;
            editTemplateViewModel.ImportType = template.ImportType;

            ImportTemplateFieldFilter fieldFilter = new ImportTemplateFieldFilter();
            fieldFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            fieldFilter.ImportTemplateID.SearchString = id;
            var templateFields = ImportTemplateField.GetImportTemplateFields(CompanyID, fieldFilter);
            if (templateFields.Count != 0)
                editTemplateViewModel.TemplateFields = new List<EditTemplateFieldViewModel>();
            foreach (var templateField in templateFields)
            {
                var templateFieldViewModel = new EditTemplateFieldViewModel();
                templateFieldViewModel.ImportTemplateFieldID = templateField.ImportTemplateFieldID;
                templateFieldViewModel.SourceField = templateField.SourceField;
                templateFieldViewModel.DestinationTable = templateField.DestinationTable;
                templateFieldViewModel.DestinationField = templateField.DestinationField;
                editTemplateViewModel.TemplateFields.Add(templateFieldViewModel);
            }
            return View(editTemplateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("ImportTemplateID,CompanyID,TemplateName,Type,ImportType,TemplateFields")] EditTemplateViewModel input)
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
                foreach (var templateField in input.TemplateFields)
                {
                    var updated = false;
                    var field = new ImportTemplateField(CompanyID, templateField.ImportTemplateFieldID);
                    if (field != null)
                    {
                        if (!field.SourceField.Equals(templateField.SourceField))
                        {
                            updated = true;
                            field.SourceField = templateField.SourceField;
                        }
                        if (!field.DestinationTable.Equals(templateField.DestinationTable))
                        {
                            updated = true;
                            field.DestinationTable = templateField.DestinationTable;
                        }
                        if (string.IsNullOrEmpty(field.DestinationField) || !field.DestinationField.Equals(templateField.DestinationField))
                        {
                            updated = true;
                            field.DestinationField = templateField.DestinationField;
                        }
                        if (updated)
                        {
                            field.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                            field.UpdatedOn = DateTime.Now;
                            field.Update();
                        }
                    }
                }
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
            ImportTemplateFieldFilter fieldFilter = new ImportTemplateFieldFilter();
            fieldFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            fieldFilter.ImportTemplateID.SearchString = id;
            var templateFields = ImportTemplateField.GetImportTemplateFields(CompanyID, fieldFilter);
            var templateFieldViewModels = new List<TemplateFieldViewModel>();
            foreach (var templateField in templateFields)
            {
                templateFieldViewModels.Add(EntityConvertViewModel(templateField));
            }
            viewModel.TemplateFields = templateFieldViewModels;
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


        public async Task<IActionResult> DownLoad(string? id, string type)
        {
            if (id == null)
                return NotFound();
            var template = new ImportTemplate(CompanyID, id);
            if (template == null)
                return NotFound();
            var fieldFilter = new ImportTemplateFieldFilter();
            fieldFilter.ImportTemplateID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            fieldFilter.ImportTemplateID.SearchString = id;
            var templateFields = ImportTemplateField.GetImportTemplateFields(CompanyID, fieldFilter);
            try
            {
                var fullNamespace = GetFullNamespace(template.Type);
                var _fileHandler = FileHandlerFactory.CreateFileHandlerInstance(fullNamespace, type);
                var fields = templateFields.Select(t => t.SourceField).ToArray();
                var fileBytes = (byte[])await _fileHandler.ExportTemplate(fields);
                var txt = Encoding.UTF8.GetString(fileBytes);
                if (type.Contains(".csv"))
                {
                    return File(fileBytes, "text/csv", $"{template.TemplateName}.csv");
                }
                else if (type.Contains(".xlsx"))
                {
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{template.TemplateName}.xlsx");
                }
                else
                {
                    return BadRequest("Unsupported file type.");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Import")]
        public async Task<IActionResult> Import(IFormFile file, string templateId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is empty");
                var template = new ImportTemplate(CompanyID, templateId);
                if (template == null)
                    return NotFound();

                var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ProgressHandler progressHandler = new ProgressHandler(ImportProgress);
                switch (template.Type)
                {
                    case "Vendor":
                        await _vendorImporter.ImportDataAsync(CompanyID, templateId, createdBy, file, progressHandler);
                        break;
                    case "Customer":
                        await _customerImporter.ImportDataAsync(CompanyID, templateId, createdBy, file, progressHandler);
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }

        private void ImportProgress(int progress, string importTemplateId)
        {
            importProgress[importTemplateId] = progress;
        }

        public IActionResult GetProgress(string importTemplateId)
        {
            var progress = importProgress.ContainsKey(importTemplateId) ? importProgress[importTemplateId] : 0;
            return Ok(progress);
        }
    }
}
