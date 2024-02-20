using InventoryStudio.Models.Templates;
using ISLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryStudio.Controllers
{
    public class TemplatesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            return View();
        }


        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImportTemplateID,CompanyID,TemplateName,Type,ImportType,UpdatedBy,UpdatedOn,CreatedBy,CreatedOn")] CreateTemplateViewModel input)
        {
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ImportTemplateID,CompanyID,TemplateName,Type,ImportType,UpdatedBy,UpdatedOn,CreatedBy,CreatedOn")] EditTemplateViewModel input)
        {
            if (id != input.ImportTemplateID)
            {
                return NotFound();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            return RedirectToAction(nameof(Index));
        }


    }
}
