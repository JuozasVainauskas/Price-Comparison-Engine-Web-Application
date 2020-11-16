using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class ConfirmPasswordController : Controller
    {
        private static string _confirmCode;
        private readonly IDatabaseManager _databaseManager;
        public ConfirmPasswordController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [AllowAnonymous]
        public IActionResult EmailConfirmation()
        {
            var code = GenerateHash.CreateSalt(16);
            code = code.Remove(code.Length - 2);

            var email = TempData["userEmail"].ToString();
            TempData["userEmail"] = email;

            _confirmCode = code;

            var sendingInformation=new SendingInformation();
            sendingInformation.ButtonPushed += (sender, e)=>
            {
                EmailSender.SendEmail(e.Code, "ernestas20111@gmail.com");
            };
            sendingInformation.Pushed(code, email);

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> EmailConfirmation(string inputCode)
        {
            if (inputCode!= null && inputCode.Equals(_confirmCode))
            {
                var email = TempData["userEmail"].ToString();
                var password = TempData["userPassword"].ToString();

                _databaseManager.RegisterUser(email, password);
                MainWindowLoggedInController.IsDeletedOrSaved = 1;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, "User")
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