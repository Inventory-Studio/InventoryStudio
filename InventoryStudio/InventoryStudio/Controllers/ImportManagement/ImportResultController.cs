using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
using InventoryStudio.FileHandlers;
using InventoryStudio.Models.ImportManagement.ImportResult;
using System.Collections.Generic;
using System.Security.Claims;


namespace InventoryStudio.Controllers.ImportManagement
{
    public class ImportResultController : Controller
    {
        private readonly string CompanyID = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FileHandlerFactory _fileHandlerFactory;
        public ImportResultController(IHttpContextAccessor httpContextAccessor, FileHandlerFactory fileHandlerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _fileHandlerFactory = fileHandlerFactory;
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
            var filter = new ImportResultFilter();
            filter.CompanyID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            filter.CompanyID.SearchString = CompanyID;
            var importResults = ImportResult.GetImportResults(filter);
            var list = new List<ImportResultViewModel>();
            foreach (var importResult in importResults)
            {
                list.Add(EntityConvertViewModel(importResult));
            }
            return View("~/Views/ImportManagement/ImportResult/Index.cshtml", list);
        }

        private ImportResultViewModel EntityConvertViewModel(ImportResult importResult)
        {
            var viewModel = new ImportResultViewModel();
            viewModel.ImportResultID = importResult.ImportResultID;
            if (!string.IsNullOrEmpty(importResult.ImportResultID))
            {
                var importTemplate = new ImportTemplate(CompanyID, importResult.ImportTemplateID);
                if (importTemplate != null)
                    viewModel.ImportTemplate = importTemplate.TemplateName;
            }
            viewModel.TotalRecords = importResult.TotalRecords;
            viewModel.SuccessfulRecords = importResult.SuccessfulRecords;
            viewModel.FailedRecords = importResult.FailedRecords;
            if (!string.IsNullOrEmpty(importResult.UploadBy))
            {
                var user = new AspNetUsers(importResult.UploadBy);
                if (user != null)
                    viewModel.UploadBy = user.UserName;
            }
            viewModel.UploadTime = importResult.UploadTime;
            viewModel.CompletionTime = importResult.CompletionTime;
            return viewModel;
        }

        private ImportFailedRecordViewModel EntityConvertViewModel(ImportFailedRecord record)
        {
            var viewModel = new ImportFailedRecordViewModel();
            viewModel.ErrorMessage = record.ErrorMessage;
            viewModel.FailedData = record.FailedData;
            return viewModel;
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();
            var importResult = new ImportResult(id);
            if (importResult == null)
                return NotFound();
            var viewModel = EntityConvertViewModel(importResult);
            var filter = new ImportFailedRecordFilter();
            filter.ImportResultID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
            filter.ImportResultID.SearchString = id;
            var importFailedRecords = ImportFailedRecord.GetImportResults(filter);
            var importFailedRecordViewModels = new List<ImportFailedRecordViewModel>();
            foreach (var importFailedRecord in importFailedRecords)
            {
                importFailedRecordViewModels.Add(EntityConvertViewModel(importFailedRecord));
            }
            viewModel.ImportFailedRecords = importFailedRecordViewModels;
            return View("~/Views/ImportManagement/ImportResult/Details.cshtml", viewModel);
        }

        private string InsertErrorMessage(string jsonString, string errorMessage)
        {
            dynamic jsonData = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, new ExpandoObjectConverter());
            var newJsonData = new ExpandoObject() as IDictionary<string, object>;
            newJsonData["ErrorMessage"] = errorMessage;
            foreach (var property in jsonData)
            {
                newJsonData[property.Key] = property.Value;
            }
            return JsonConvert.SerializeObject(newJsonData);
        }

        public async Task<IActionResult> DownLoad(string id, string type)
        {
            try
            {
                var filter = new ImportFailedRecordFilter();
                filter.ImportResultID = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.ImportResultID.SearchString = id;
                var importFailedRecords = ImportFailedRecord.GetImportResults(filter);
                var list = importFailedRecords
                    .Select(r => (r.ErrorMessage, r.FailedData))
                    .ToList();
                var jsons = new List<string>();
                foreach (var json in list)
                {
                    string jsonStringWithErrorMessage = InsertErrorMessage(json.FailedData, json.ErrorMessage);
                    jsons.Add(jsonStringWithErrorMessage);
                }
                var _fileHandler = _fileHandlerFactory.CreateFileHandler(type);
                byte[] fileBytes = await _fileHandler.ExportImportResult(jsons);
                if (type.Contains(".csv"))
                {
                    return File(fileBytes, "text/csv", "data.csv");
                }
                else if (type.Contains(".xlsx"))
                {
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "filename.xlsx");
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
    }
}
