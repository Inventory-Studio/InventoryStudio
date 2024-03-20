using InventoryStudio.FileHandlers;
using InventoryStudio.Models;
using InventoryStudio.Models.Templates;
using InventoryStudio.Services.FileHandlers;
using InventoryStudio.Services.Importers;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using ISLibrary.OrderManagement;
using ISLibrary.Template;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SendGrid.Helpers.Mail;
using System.Security.Claims;
using System.Text;

namespace InventoryStudio.Controllers
{
    public class TemplatesController : BaseController
    {
        private readonly string CompanyID = string.Empty;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private static Dictionary<string, int> importProgress = new Dictionary<string, int>();

        private readonly ImporterFactory _importerFactory;
        private readonly FileHandlerFactory _fileHandlerFactory;

        public TemplatesController(IHttpContextAccessor httpContextAccessor, ImporterFactory importerFactory, FileHandlerFactory fileHandlerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                Claim? company = user.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                    CompanyID = company.Value;
            }
            _importerFactory = importerFactory;
            _fileHandlerFactory = fileHandlerFactory;
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
            return View();
        }

        private string GetFullNamespace(string typeName)
        {
            return $"ISLibrary.Template.{typeName}Template";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTemplateViewModel input)
        {
            if (ModelState.IsValid && input.File != null && input.File.Length > 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new ImportTemplate();
                template.CompanyID = CompanyID;
                template.TemplateName = input.TemplateName;
                template.Type = input.Type.ToString();
                template.ImportType = input.ImportType.ToString();
                template.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                template.Create();

                var fileType = Path.GetExtension(input.File.FileName);
                var _fileHandler = _fileHandlerFactory.CreateFileHandler(fileType);
                var headers = await _fileHandler.ImportTemplate(input.File);
                if (headers.Count == 0)
                    throw new Exception();

                Dictionary<string, string> mappings;
                var type = input.Type.ToString();
                List<ImportTemplateField> importTemplateFields = new List<ImportTemplateField>();
                if (type == "Item")
                {
                    mappings = MapHeadersToEntityProperties<ItemTemplate>(headers[0]);
                    importTemplateFields.AddRange(GetTemplateFields(template.ImportTemplateID, CompanyID, userId, "Item", mappings));
                }
                else if (type == "Vendor")
                {
                    mappings = mappings = MapHeadersToEntityProperties<VendorTemplate>(headers[0]);
                    importTemplateFields.AddRange(GetTemplateFields(template.ImportTemplateID, CompanyID, userId, "Vendor", mappings));
                }
                else if (type == "Customer")
                {
                    mappings = MapHeadersToEntityProperties<CustomerTemplate>(headers[0]);
                    importTemplateFields.AddRange(GetTemplateFields(template.ImportTemplateID, CompanyID, userId, "Customer", mappings));
                }
                else if (type == "SalesOrder")
                {
                    //SalesOrder
                    mappings = MapHeadersToEntityProperties<SalesOrderTemplate>(headers[0]);
                    if (mappings.Count != 0)
                    {
                        var salesOrderFields = GetTemplateFields(template.ImportTemplateID, CompanyID, userId, "SalesOrder", mappings);
                        importTemplateFields.AddRange(salesOrderFields);
                    }

                    //SalesOrderLine
                    mappings = MapHeadersToEntityProperties<SalesOrderLineTemplate>(headers[1]);
                    if (mappings.Count != 0)
                    {
                        //importTemplateFields.AddRange(GetTemplateFields(template.ImportTemplateID, input.CompanyID, "SalesOrderLine", mappings));
                        var salesOrderLineFields = GetTemplateFields(template.ImportTemplateID, CompanyID, userId, "SalesOrderLine", mappings);
                        importTemplateFields.AddRange(salesOrderLineFields);
                    }

                    //SalesOrderLineDetail
                    mappings = MapHeadersToEntityProperties<SalesOrderLineDetailTemplate>(headers[2]);
                    if (mappings.Count != 0)
                    {
                        //importTemplateFields.AddRange(GetTemplateFields(template.ImportTemplateID, input.CompanyID, "SalesOrderLineDetail", mappings));
                        var salesOrderLineDetailFields = GetTemplateFields(template.ImportTemplateID, CompanyID, userId, "SalesOrderLineDetail", mappings);
                        importTemplateFields.AddRange(salesOrderLineDetailFields);
                    }
                }

                foreach (var importTemplateField in importTemplateFields)
                {
                    importTemplateField.Create();
                }

                //【Todo】
                //var mappings = MapHeadersToEntityProperties<ItemTemplate>(headers[0]);
                //foreach (var mapping in mappings)
                //{
                //    var templateField = new ImportTemplateField();
                //    templateField.ImportTemplateID = template.ImportTemplateID;
                //    templateField.CompanyID = input.CompanyID;
                //    templateField.SourceField = mapping.Key;
                //    templateField.DestinationTable = nameof(Customer);
                //    templateField.DestinationField = mapping.Value;
                //    templateField.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //    templateField.CreatedOn = DateTime.Now;
                //    templateField.Create();
                //}
                return RedirectToAction(nameof(Index));
            }

            return View(input);
        }

        private List<ImportTemplateField> GetTemplateFields(string importTemplateId, string companyId, string userId, string table, Dictionary<string, string> mappings)
        {
            var templateFields = new List<ImportTemplateField>();
            foreach (var mapping in mappings)
            {
                var templateField = new ImportTemplateField();
                templateField.ImportTemplateID = importTemplateId;
                templateField.CompanyID = companyId;
                templateField.SourceField = mapping.Key;
                templateField.DestinationTable = table;
                templateField.DestinationField = mapping.Value;
                templateField.CreatedBy = userId;
                templateField.CreatedOn = DateTime.Now;
                templateFields.Add(templateField);
            }
            return templateFields;
        }

        private Dictionary<string, string> MapHeadersToEntityProperties<T>(string[] headerFields)
        {
            var entityTypeProperties = typeof(T).GetProperties()
                .Where(p => (!p.PropertyType.IsGenericType || p.PropertyType.GetGenericTypeDefinition() != typeof(List<>)) && p.PropertyType.Namespace.StartsWith(nameof(System)))
                .Select(p => p.Name).ToList();
            var mapping = new Dictionary<string, string?>();
            foreach (var entityTypeProperty in entityTypeProperties)
            {
                if (headerFields.Contains(entityTypeProperty))
                {
                    mapping[entityTypeProperty] = entityTypeProperty;
                }
                else
                {
                    mapping[entityTypeProperty] = null;
                }
            }
            return mapping;
        }
        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();
            var template = new ImportTemplate(CompanyID, id);
            if (template == null)
                return NotFound();
            var editTemplateViewModel = new EditTemplateViewModel();
            editTemplateViewModel.ImportTemplateID = template.ImportTemplateID;
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
        public IActionResult Edit(EditTemplateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var template = new ImportTemplate(CompanyID, input.ImportTemplateID);
                if (template == null)
                    return NotFound();
                template.CompanyID = CompanyID;
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
                importProgress[templateId] = 0;
                var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ProgressHandler progressHandler = new ProgressHandler(ImportProgress);
                var fileType = Path.GetExtension(file.FileName);
                var filehanlder = _fileHandlerFactory.CreateFileHandler(fileType);
                var importer = _importerFactory.CreateImporter(template.Type);
                if (template.Type == "SalesOrder")
                {
                    var datas = await filehanlder.ImportDatas(file);
                    SalesOrderImporter salesOrderImporter = (SalesOrderImporter)importer;
                    await salesOrderImporter.ImportDatasAsync(CompanyID, templateId, createdBy, progressHandler, datas);
                }
                else
                {
                    var datas = await filehanlder.ImportData(file);
                    await importer.ImportDataAsync(CompanyID, templateId, createdBy, progressHandler, datas);
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
