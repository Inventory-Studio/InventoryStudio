using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Syncfusion.EJ2.Base;
using InventoryStudio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using ISLibrary;
using InventoryStudio.Models.Account;
using InventoryStudio.Models.ViewModels;
using ISLibrary.AspNet;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using InventoryStudio.Data;

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

        public IActionResult Index()
        {
            var users = new List<AspNetUsers>();
            var company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company != null)
                users = AspNetUsers.GetAspNetUserss(company.Value);
            _userIndexViewModel.Users = users;
            return View("~/Views/Account/User/Index.cshtml", _userIndexViewModel);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            return View("~/Views/Account/User/Details.cshtml", user);
        }

        public IActionResult Create()
        {
            CreateUserViewModel viewModel = new CreateUserViewModel();
            return View("~/Views/Account/User/Create.cshtml", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel input)
        {
            var user = CreateUser();
            user.Status = input.Status;
            user.UserType = "Normal";
            user.UserName = input.UserName;
            user.Email = input.Email;
            user.PhoneNumber = input.PhoneNumber;
            await _userStore.SetUserNameAsync(user, input.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, input.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl = Url.Content("~/") },
                    protocol: Request.Scheme);
                if (callbackUrl != null)
                {
                    await _emailSender.SendEmailAsync(input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                }

                var company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");

                AspNetUserCompany aspNetUserCompany = new AspNetUserCompany();
                aspNetUserCompany.UserId = userId;
                aspNetUserCompany.CompanyID = company.Value;
                aspNetUserCompany.Create();

                await _userManager.AddClaimAsync(user, new Claim("CompanyId", company.Value));

                //user.UserName = input.UserName;
                //await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }

            return Redirect("/Users");
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                                                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<User>)_userStore;
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            var editViewModel = new EditUserViewModel
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
                if (currentUserRoles.Any())
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, EditUserViewModel input)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id)
            {
                UserName = input.UserName,
                Status = input.Status,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber
            };
            user.Update();

            if (user.Roles != null)
            {
                var currentUserRoles = user.Roles.Select(t => t.Id).ToList();

                var removeRoleIds = currentUserRoles.Except(input.SelectedRoles).ToList();
                foreach (var selectedRoleId in input.SelectedRoles)
                {
                    var userRole = new AspNetUserRoles(user.Id, selectedRoleId);
                    userRole.Update();
                }

                foreach (var removeRoleId in removeRoleIds)
                {
                    var userRole = new AspNetUserRoles(user.Id, removeRoleId);
                    userRole.Delete();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            return View("~/Views/Account/User/Delete.cshtml", user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            user.Delete();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Invite()
        {
            return View("~/Views/Account/User/Invite.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendInvitation(InviteUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "This email is already registered and cannot send invitations";
                    return RedirectToAction("Invite", "User");
                }

                string inviteCode = Guid.NewGuid().ToString("N").Substring(0, 16);
                AspNetUserInvites invite = new()
                {
                    Email = model.Email,
                    Code = inviteCode,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                invite.Create();

                string inviteUrl = Url.Action("Register", "Account", new { area = "Identity", inviteCode }, Request.Scheme) ??
                                   "";
                await _emailSender.SendEmailAsync(model.Email, "Invitation to join",
                    $"Please click the following link to accept the invitation: {inviteUrl}");

                TempData["StatusMessage"] = "Invitation sent successfully.";
                return RedirectToAction("Invite", "User");
            }

            return View("~/Views/Account/User/Invite.cshtml", model);
        }


        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<AspNetUsers> dataSource = _userIndexViewModel.Users.AsEnumerable();
            DataOperations operation = new DataOperations();
            int totalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                var company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                {
                    AspNetUsersFilter aspNetUsersFilter = new();
                    dataSource = AspNetUsers.GetAspNetUserss(company.Value, aspNetUsersFilter, dm.Take,
                        (dm.Skip / dm.Take) + 1, out totalRecord).AsEnumerable();
                }
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                dataSource = operation.PerformSorting(dataSource, dm.Sorted);
            }

            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                dataSource = operation.PerformFiltering(dataSource, dm.Where, dm.Where[0].Operator);
            }

            return dm.RequiresCounts ? Json(new { result = dataSource, count = totalRecord }) : Json(dataSource);
        }
    }
}