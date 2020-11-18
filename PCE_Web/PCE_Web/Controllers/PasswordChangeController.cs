using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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
    public class PasswordChangeController : Controller
    {
        private readonly IDatabaseManager _databaseManager;

        public PasswordChangeController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        //public class InputModel
        //{
        //    [Display(Name = "Email")]
        //    [DataType(DataType.EmailAddress)]
        //    [Required(ErrorMessage = "Turite įrašyti email.")]
        //    [EmailSpelling]
        //    [EmailExistence]
        //    public string Email { get; set; }

        //    [Display(Name = "Code")]
        //    [DataType(DataType.Text)]
        //    [Required(ErrorMessage = "Turite įvesti kodą, nusiųsta į jūsų email paštą.")]
        //    public string Code { get; set; }

        //    [Display(Name = "Password")]
        //    [DataType(DataType.Password)]
        //    [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
        //    [StringLength(100, MinimumLength = 4, ErrorMessage = "Slaptažodis turi būti bet 4 simbolių ilgio.")]
        //    [PasswordSpelling]
        //    public string Password { get; set; }

        //    [Display(Name = "Confirm password")]
        //    [DataType(DataType.Password)]
        //    [Required(ErrorMessage = "Turite patvirtinti slaptažodį.")]
        //    [Compare("Password", ErrorMessage = "Slaptažodis turi sutapti su patvirtinimo slaptažodžiu.")]
        //    public string ConfirmPassword { get; set; }
        //}

        [AllowAnonymous]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(InputModel input)
        {
            if (input.EmailModel != null)
            {
                if (ModelState.IsValid)
                {
                    var confirmCode = GenerateHash.CreateSalt(16);
                    confirmCode = confirmCode.Remove(confirmCode.Length - 2);
                    var email = input.EmailModel.Email;

                    EmailSender.SendEmail(confirmCode, "ernestas20111@gmail.com");

                    TempData["tempEmail"] = email;
                    TempData["tempCode"] = confirmCode;
                }
            }
            else if (input.CodeModel != null)
            {
                var confirmCode = TempData["tempCode"].ToString();
                TempData["tempCode"] = confirmCode;

                if (ModelState.IsValid)
                {
                    if (input.CodeModel.Code.Equals(confirmCode))
                    {
                        return View();
                    }
                    else
                    {
                        ViewBag.ShowMessage = true;
                    }
                }
            }
            else
            {
                if (ModelState.IsValid)
                {

                }
            }
            return View();
        }
    }
}