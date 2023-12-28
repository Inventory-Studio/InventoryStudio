using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryStudio.Data;
using InventoryStudio.Models;
using ISLibrary.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using ISLibrary;

namespace InventoryStudio.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserController> _logger;
        public UserController(
            SignInManager<User> signInManager,
           UserManager<User> userManager,
           IUserStore<User> userStore,
           IEmailSender emailSender,
           ILogger<UserController> logger)
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
            var users = ISLibrary.Account.UserInfo.GetUsers();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();
            var user = new UserInfo(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUser input)
        {
            var user = CreateUser();
            user.Status = input.Status;
            user.UserType = input.UserType;
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
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = Url.Content("~/") },
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

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound();
            var user = new UserInfo(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id || id == 0)
                return NotFound();
            var current = new UserInfo(id);
            if (current == null)
                return NotFound();
            current.UserName = user.UserName;
            current.Status = user.Status;
            current.UserType = user.UserType;
            current.Email = user.Email;
            current.EmailConfirmed = user.EmailConfirmed;
            current.PhoneNumber = user.PhoneNumber;
            current.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            current.TwoFactorEnabled = user.TwoFactorEnabled;
            current.LockoutEnabled = user.LockoutEnabled;
            current.LockoutEnd = user.LockoutEnd;
            current.AccessFailedCount = user.AccessFailedCount;
            current.Update();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();
            var user = new UserInfo(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = new UserInfo(id);
            if (user == null)
                return NotFound();
            user.Delete();
            return RedirectToAction(nameof(Index));
        }
    }
}
