using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class LoggingController : Controller
    {
        private readonly IDatabaseManager _databaseManager;

        public LoggingController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public class InputModel
        {
            [Display(Name = "Email")]
            [DataType(DataType.EmailAddress)]
            [Required(ErrorMessage = "Turite įrašyti email.")]
            [UserExistence]
            public string Email { get; set; }
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
            public string Password { get; set; }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(InputModel input)
        {
            if (ModelState.IsValid)
            {
                var user = _databaseManager.LoginUser(input.Email, input.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, input.Email),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

                    MainWindowLoggedInController.IsDeletedOrSaved = 1;
                    return RedirectToAction("Items", "MainWindowLoggedIn");
                }
                else
                {
                    ViewBag.ShowMessage = true;
                }
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult SendCode(string email)
        {

            var code = GenerateHash.CreateSalt(16);
            code = code.Remove(code.Length - 2);
            EmailSender.SendEmail(code, email);

            TempData["tempEmail"] = email;
            TempData["tempCode"] = code;
            return RedirectToAction("Login", "Logging");
        }

        [AllowAnonymous]
        public IActionResult ConfirmEmail(string confirmCode)
        {
            var email = TempData["tempEmail"].ToString();
            var code = TempData["tempCode"].ToString();
            TempData["tempEmail"] = email;
            TempData["tempCode"] = code;

            return RedirectToAction("Login", "Logging");
        }

        [AllowAnonymous]
        public IActionResult ChangePassword(string password, string confirmPassword)
        {
            return RedirectToAction("Login", "Logging");
        }
    }
}
