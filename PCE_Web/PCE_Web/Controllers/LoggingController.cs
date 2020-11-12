using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
                    //var userId = Guid.NewGuid().ToString();
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimTypes.Name, userId),
                    //    new Claim("access_token", GetAccessToken(userId))
                    //};

                    //var claimsIdentity = new ClaimsIdentity(
                    //    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //var authProperties = new AuthenticationProperties();

                    //await HttpContext.SignInAsync(
                    //    CookieAuthenticationDefaults.AuthenticationScheme,
                    //    new ClaimsPrincipal(claimsIdentity),
                    //    authProperties);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, input.Email)
                    };
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimTypes.Name, Guid.NewGuid().ToString())
                    //};
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

        //private static string GetAccessToken(string userId)
        //{
        //    const string issuer = "localhost";
        //    const string audience = "localhost";

        //    var identity = new ClaimsIdentity(new List<Claim>
        //    {
        //        new Claim("sub", userId)
        //    });

        //    var bytes = Encoding.UTF8.GetBytes(userId);
        //    var key = new SymmetricSecurityKey(bytes);
        //    var signingCredentials = new SigningCredentials(
        //        key, SecurityAlgorithms.HmacSha256);

        //    var now = DateTime.UtcNow;
        //    var handler = new JwtSecurityTokenHandler();

        //    var token = handler.CreateJwtSecurityToken(
        //        issuer, audience, identity,
        //        now, now.Add(TimeSpan.FromHours(1)),
        //        now, signingCredentials);

        //    return handler.WriteToken(token);
        //}

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
