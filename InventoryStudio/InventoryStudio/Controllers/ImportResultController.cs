using InventoryStudio.Models.ImportResults;
using InventoryStudio.Services.File;
using ISLibrary;
using ISLibrary.ImportTemplateManagement;
using Microsoft.AspNetCore.Mvc;

namespace InventoryStudio.Controllers
{
    public class ImportResultController : Controller
    {
        private readonly string CompanyID = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExportService _exportService;
        public ImportResultController(IHttpContextAccessor httpContextAccessor, ExportService exportService)
        {
            _httpContextAccessor = httpContextAccessor;
            _exportService = exportService;
        }

        public IActionResult Index()
        {
            //【Todo】 Need to add filtering conditions
            var importResults = ImportResult.GetImportResults(new ImportResultFilter());
            var list = new List<ImportResultViewModel>();
            foreach (var importResult in importResults)
            {
                list.Add(EntityConvertViewModel(importResult));
            }
            return View(list);
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
            return View(viewModel);
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
                    string jsonStringWithErrorMessage = _exportService.InsertErrorMessage(json.FailedData, json.ErrorMessage);
                    jsons.Add(jsonStringWithErrorMessage);
                }
                byte[] fileBytes;
                if (type.Contains(".csv"))
                {
                    fileBytes = await _exportService.ExportJsonToCsv(jsons);
                    return File(fileBytes, "text/csv", "data.csv");
                }
                else if (type.Contains(".xlsx"))
                {
                    fileBytes = await _exportService.ExportJsonToExcel(jsons);
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
