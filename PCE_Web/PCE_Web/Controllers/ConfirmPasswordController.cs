using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class ConfirmPasswordController : Controller
    {
        private static string _confirmCode;

        public IActionResult EmailConfirmation()
        {
            var code = GenerateHash.CreateSalt(16);
            code = code.Remove(code.Length - 2);
            new SendEmail(code, "ernestas20111@gmail.com");

            _confirmCode = code;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailConfirmation(string inputCode)
        {
            if (inputCode!= null && inputCode.Equals(_confirmCode))
            {
                DatabaseManager.RegisterUser(RegistrationController.Email, RegistrationController.Password);
                MainWindowLoggedInController.EmailCurrentUser = RegistrationController.Email;
                MainWindowLoggedInController.IsDeletedOrSaved = 1;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, RegistrationController.Email)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

                return RedirectToAction("Items", "MainWindowLoggedIn");
            }
            else
            {
                ViewBag.ShowMessage = true;
            }
            return View();
        }
    }
}