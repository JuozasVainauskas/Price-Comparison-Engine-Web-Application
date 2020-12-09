using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class ConfirmPasswordController : Controller
    {
        private static string _confirmCode;
        private readonly IAccountManager _accountManager;
        private readonly IEmailSenderInterface _emailSender;

        public ConfirmPasswordController(IAccountManager accountManager, IEmailSenderInterface emailSender)
        {
            _accountManager = accountManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        public IActionResult EmailConfirmation()
        {
            var code = GenerateHash.CreateSalt(16);
            code = code.Remove(code.Length - 2);
            _confirmCode = code;

            var email = TempData["userEmail"].ToString();
            TempData["userEmail"] = email;

            var sendingInformation=new SendingInformation();
            sendingInformation.ButtonPushed += (sender, e)=>
            {
                _emailSender.SendEmail(e.Code, e.Email);
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

                _accountManager.RegisterUser(email, password);
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