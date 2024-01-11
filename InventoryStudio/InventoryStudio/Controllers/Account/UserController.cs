using System;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index()
        {
            var users = new List<AspNetUsers>();
            var company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
            if (company != null)
                users = AspNetUsers.GetAspNetUserss(company.Value);
            _userIndexViewModel.Users = users;
            return View("~/Views/Account/User/Index.cshtml", _userIndexViewModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            if (user == null)
                return NotFound();
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
            await _userStore.SetUserNameAsync(user, input.Email, CancellationToken.None);
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

                await _emailSender.SendEmailAsync(input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                user.UserName = input.UserName;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View();
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

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            if (user == null)
                return NotFound();
            var editViewModel = new EditUserViewModel();
            editViewModel.Id = user.Id;
            editViewModel.UserName = user.UserName;
            editViewModel.Status = user.Status;
            editViewModel.Email = user.Email;
            editViewModel.PhoneNumber = user.PhoneNumber;

            var currentUserRoles = user.Roles.ToList();
            if (currentUserRoles.Any())
                editViewModel.SelectedRoles = currentUserRoles.Select(t => t.Id).ToList();
            var companyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var allRoles = AspNetRoles.GetAspNetRoless(companyId.Value).Select(t => new SelectListItem
            {
                Value = t.Id,
                Text = t.Name,
                Selected = editViewModel.SelectedRoles.Contains(t.Name)
            }).ToList();
            editViewModel.AllRoles = allRoles;
            return View("~/Views/Account/User/Edit.cshtml", editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel input)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            if (user == null)
                return NotFound();
            user.UserName = input.UserName;
            user.Status = input.Status;
            user.Email = input.Email;
            user.PhoneNumber = input.PhoneNumber;
            user.Update();

            var currentUserRoles = user.Roles.Select(t => t.Id).ToList();

            var removeRoleIds = currentUserRoles.Except(input.SelectedRoles).ToList();
            foreach (var selectedRoleId in input.SelectedRoles)
            {
                var userRole = new AspNetUserRoles(user.Id, selectedRoleId);
                if (userRole != null)
                    userRole.Update();
                else
                {
                    userRole = new AspNetUserRoles();
                    userRole.UserId = user.Id;
                    userRole.RoleId = selectedRoleId;
                    userRole.Create();
                }
            }

            foreach (var removeRoleId in removeRoleIds)
            {
                var userRole = new AspNetUserRoles(user.Id, removeRoleId);
                if (userRole != null)
                    userRole.Delete();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            if (user == null)
                return NotFound();
            return View("~/Views/Account/User/Delete.cshtml", user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = new AspNetUsers(id);
            if (user == null)
                return NotFound();
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
                AspNetUserInvites invite = new AspNetUserInvites();
                invite.Email = model.Email;
                invite.Code = inviteCode;
                invite.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                invite.Create();

                string inviteUrl = Url.Action("Register", "Account", new { area = "Identity", inviteCode = inviteCode }, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Invitation to join", $"Please click the following link to accept the invitation: {inviteUrl}");

                TempData["StatusMessage"] = "Invitation sent successfully.";
                return RedirectToAction("Invite", "User");
            }

            return View("~/Views/Account/User/Invite.cshtml", model);
        }


        public IActionResult UrlDataSource([FromBody] DataManagerRequest dm)
        {
            IEnumerable<AspNetUsers> dataSource = _userIndexViewModel.Users.AsEnumerable();
            DataOperations operation = new DataOperations();
            int TotalRecord = 0;
            if (dm.Skip != 0 || dm.Take != 0)
            {
                CompanyFilter companyFilter = new();
                var company = User.Claims.FirstOrDefault(t => t.Type == "CompanyId");
                if (company != null)
                    dataSource = AspNetUsers.GetAspNetUserss(company.Value, null, dm.Take, (dm.Skip / dm.Take) + 1, out TotalRecord).AsEnumerable();

            }

            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                dataSource = operation.PerformSorting(dataSource, dm.Sorted);
            }

            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                //dm.Where[0].Field = Regex.Replace(dm.Where[0].predicates[0].Field, @"\b\p{Ll}", match => match.Value.ToUpper());
                dataSource = operation.PerformFiltering
                    (dataSource, dm.Where, dm.Where[0].Operator);
            }

            return dm.RequiresCounts ? Json(new { result = dataSource, count = TotalRecord }) : Json(dataSource);
        }

    }
}
