using InventoryStudio.Models;
using InventoryStudio.Models.ViewModels;
using InventoryStudio.Services.Authorization;
using ISLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Linq;

namespace InventoryStudio.Controllers;

public class BaseController : Controller
{
    // private readonly UserManager<User> _userManager;
    // private readonly IAuthorizationService _authorizationService;
    public MainLayoutViewModel? mainLayoutViewModel { get; private set; }

    public BaseController()
    {
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        mainLayoutViewModel = SetupLayoutViewModel();
        ViewBag.MainLayoutViewModel = mainLayoutViewModel ?? new MainLayoutViewModel();
        base.OnActionExecuting(filterContext);
    }

    private MainLayoutViewModel SetupLayoutViewModel()
    {
        var userId = User?.Claims.FirstOrDefault(t =>
            t.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        var companyId = User?.Claims?.FirstOrDefault(t => t.Type == "CompanyId");

        if (userId == null)
        {
            return new MainLayoutViewModel();
        }

        var user = new AspNetUsers(userId?.Value ?? "");
        var company = new Company(companyId?.Value ?? "");
        return new MainLayoutViewModel()
        {
            FullName = user.UserName,
            CurrentCompany = company.CompanyName
        };
    }
}