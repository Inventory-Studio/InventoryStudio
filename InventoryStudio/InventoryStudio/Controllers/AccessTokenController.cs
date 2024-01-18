using Azure.Core;
using InventoryStudio.Models.AccessToken;
using ISLibrary.AspNet;
using Microsoft.AspNetCore.Mvc;

namespace InventoryStudio.Controllers
{
    public class AccessTokenController : Controller
    {
        public IActionResult Index()
        {
            var accessTokens = AspNetAccessTokens.GetAspNetUserAccessTokens();
            return View(accessTokens);
        }

        public IActionResult Details(string id)
        {
            var accessToken = new AspNetAccessTokens(id);
            return View(accessToken);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationName,TokenName,InActive,RoleId")] CreateAccessTokenViewModel input)
        {
            return View();
        }

        public IActionResult Edit(string id)
        {
            var accessToken = new AspNetAccessTokens(id);
            return View(accessToken);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ApplicationName,TokenName,InActive,RoleId")] EditAccessTokenViewModel input)
        {
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var accessToken = new AspNetAccessTokens(id);
            return View(accessToken);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
