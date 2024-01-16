using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using InventoryStudio.Models;
using InventoryStudio.Models.Account;
using InventoryStudio.Models.ViewModels;
using ISLibrary;
using ISLibrary.AspNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Syncfusion.EJ2.Base;

namespace InventoryStudio.Controllers.Account
{
    public class UserController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserController> _logger;

        private UserIndexViewModel _userIndexViewModel = new() { Users = new List<AspNetUsers>() };

        public UserController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            IEmailSender emailSender,
            ILogger<UserController> logger
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
            _logger = logger;
        }

        [Authorize(Policy = "Account-User-List")]
        public IActionResult Index()
        {
            List<AspNetUsers> users = new();
            Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company != null)
            {
                users = AspNetUsers.GetAspNetUserss(company.Value);
            }

            _userIndexViewModel.Users = users;
            return View("~/Views/Account/User/Index.cshtml", _userIndexViewModel);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            AspNetUsers user = new(id);
            return View("~/Views/Account/User/Details.cshtml", user);
        }

        [Authorize(Policy = "Account-User-Create")]
        public IActionResult Create()
        {
            CreateUserViewModel viewModel = new();
            return View("~/Views/Account/User/Create.cshtml", viewModel);
        }

        [Authorize(Policy = "Account-User-Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel input)
        {
            User user = CreateUser();
            user.Status = input.Status;
            user.UserType = "Normal";
            user.UserName = input.UserName;
            user.Email = input.Email;
            user.PhoneNumber = input.PhoneNumber;
            await _userStore.SetUserNameAsync(user, input.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, input.Email, CancellationToken.None);
            IdentityResult result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                string userId = await _userManager.GetUserIdAsync(user);
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string? callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new
                    {
                        area = "Identity",
                        userId,
                        code,
                        returnUrl = Url.Content("~/")
                    },
                    protocol: Request.Scheme
                );
                if (callbackUrl != null)
                {
                    await _emailSender.SendEmailAsync(
                        input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                    );
                }

                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");

                AspNetUserCompany aspNetUserCompany = new()
                {
                    UserId = userId,
                    CompanyID = company?.Value ?? ""
                };
                aspNetUserCompany.Create();

                await _userManager.AddClaimAsync(user, new Claim("CompanyId", company?.Value ?? ""));

                //user.UserName = input.UserName;
                //await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }

            return Redirect("/User");
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(User)}'. "
                    + $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively "
                    + $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml"
                );
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            return !_userManager.SupportsUserEmail
                ? throw new NotSupportedException(
                    "The default UI requires a user store with email support."
                )
                : (IUserEmailStore<User>)_userStore;
        }

        [Authorize(Policy = "Account-User-Edit")]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            AspNetUsers user = new(id);
            EditUserViewModel editViewModel =
                new()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Status = user.Status,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
            if (user.Roles != null)
            {
                var currentUserRoles = user.Roles.ToList();
                editViewModel.SelectedRoles = currentUserRoles.Select(t => t.Id).ToList();
                var companyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
                if (companyId != null)
                {
                    var allRoles = AspNetRoles.GetAspNetRoless(companyId.Value).Select(t => new SelectListItem
                    {
                        Value = t.Id,
                        Text = t.Name,
                        Selected = editViewModel.SelectedRoles.Contains(t.Name)
                    }).ToList();
                    editViewModel.AllRoles = allRoles;
                }
            }

            return View("~/Views/Account/User/Edit.cshtml", editViewModel);
        }

        [Authorize(Policy = "Account-User-Edit")]
        [HttpPost]
        public IActionResult Edit(string id, EditUserViewModel input)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            AspNetUsers user =
                new(id)
                {
                    UserName = input.UserName,
                    Status = input.Status,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber
                };
            _ = user.Update();


            List<string> selectedRoles = input?.SelectedRoles ?? new List<string>();

            List<string> currentUserRoles =
                user.Roles != null ? user.Roles.Select(t => t.Id).ToList() : new List<string>();

            List<string> removeRoleIds = currentUserRoles.Any()
                ? currentUserRoles.Except(selectedRoles).ToList()
                : new List<string>();

            foreach (string selectedRoleId in selectedRoles)
            {
                if (!currentUserRoles.Contains(selectedRoleId))
                {
                    AspNetUserRoles userRole = new AspNetUserRoles();
                    userRole.UserId = user.Id;
                    userRole.RoleId = selectedRoleId;
                    userRole.Create();
                    //Create without loading data, so use an empty constructor 
                    //new(user.Id, selectedRoleId);
                    //_ = userRole.Create();
                }
            }

            foreach (string? removeRoleId in removeRoleIds)
            {
                AspNetUserRoles userRole = new(user.Id, removeRoleId);
                _ = userRole.Delete();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Account-User-Delete")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            AspNetUsers user = new(id);
            return View("~/Views/Account/User/Delete.cshtml", user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            AspNetUsers user = new(id);
            _ = user.Delete();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Account-User-Invite")]
        public IActionResult Invite()
        {
            InviteUserViewModel inviteUserViewModel = new();
            return View("~/Views/Account/User/Invite.cshtml", inviteUserViewModel);
        }

        [Authorize(Policy = "Account-User-Invite")]
        [HttpPost]
        public async Task<IActionResult> SendInvitation(InviteUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    TempData["Message"] =
                        "This email is already registered and cannot send invitations";
                    return RedirectToAction("Invite", "User");
                }

                AspNetUserInvitesFilter filter = new AspNetUserInvitesFilter();
                filter.Email = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.Email.SearchString = model.Email;
                filter.UserId = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.UserId.SearchString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                filter.IsAccepted = new CLRFramework.Database.Filter.StringSearch.SearchFilter();
                filter.IsAccepted.SearchString = "0";
                var existingInvite = AspNetUserInvites.GetAspNetUserInvites(filter);
                if (existingInvite != null && existingInvite.Count > 0)
                {
                    TempData["Message"] =
                       "This email sends an invitation";
                    return RedirectToAction("Invite", "User");
                }

                string inviteCode = Guid.NewGuid().ToString("N")[..16];
                AspNetUserInvites invite =
                    new()
                    {
                        Email = model.Email,
                        Code = inviteCode,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };
                invite.Create();
                // ReSharper disable once Mvc.ActionNotResolved
                //string inviteUrl =
                //    Url.Action(
                //        "Register",
                //        "Account",
                //        new { area = "Identity", inviteCode },
                //        Request.Scheme
                //    ) ?? "";

                var inviteUrl = Url.Page(
                       "/Account/Register",
                       pageHandler: null,
                       values: new { area = "Identity", inviteCode },
                       protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Invitation to join",
                    $"Please click the following link to accept the invitation: {inviteUrl}"
                );

                TempData["Message"] = "Invitation sent successfully.";
                return RedirectToAction("Invite", "User");
            }

            return View("~/Views/Account/User/Invite.cshtml", model);
        }

        public IActionResult Insert([FromBody] CRUDModel<AspNetUsers> value)
        {
            return Json(value.Value);
        }

        public IActionResult Update([FromBody] CRUDModel<AspNetUsers> value)
        {
            return Json(value.Value ?? new AspNetUsers());
        }

        public IActionResult Remove([FromBody] CRUDModel<AspNetUsers> value)
        {
            if (value.Key != null)
            {
                DeleteConfirmed(value.Key.ToString() ?? "");
            }

            return Json(value);
        }

        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<AspNetUsers> dataSource = new List<AspNetUsers>().AsEnumerable();
            DataOperations operation = new();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                Claim? company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    AspNetUsersFilter aspNetUsersFilter = new();
                    _userIndexViewModel.Users = AspNetUsers.GetAspNetUserss(
                        company.Value,
                        aspNetUsersFilter,
                        dm.Take,
                        (dm.Skip / dm.Take) + 1,
                        out totalRecord
                    );
                    dataSource = _userIndexViewModel.Users.AsEnumerable();
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