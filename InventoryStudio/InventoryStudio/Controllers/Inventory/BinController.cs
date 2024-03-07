using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ISLibrary;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using Newtonsoft.Json;
using NPOI.POIFS.NIO;

namespace InventoryStudio.Controllers
{
    public class BinController : BaseController

    { 
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public BinController(UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }
  
        [Authorize(Policy = "Inventory-Bin-Create")]
        [HttpPost]
        public IActionResult Create(Bin bin)
        {
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (organizationClaim == null)
            {
                return Json(new { error = "Invalid organization information." });
            }

            Bin objBin = new Bin();
            bin.CreatedBy = Convert.ToString(_userManager.GetUserId(User));
            bin.CompanyID = organizationClaim.Value;
            try
            {
                bin.Create();
                return Json(new { succcess = "Create Successed" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [Authorize(Policy = "Inventory-Bin-Delete")]
        public IActionResult Delete(string BinID)
        {           
            var organizationClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var objBin = new Bin(organizationClaim.Value,BinID);

            if (objBin == null)
            {
                return Json(new { error = "Not Found" });
            }

            try
            {
                objBin.Delete();
                return Json(new { succcess = "Delete Successed" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
           
        
        }

    }
}
