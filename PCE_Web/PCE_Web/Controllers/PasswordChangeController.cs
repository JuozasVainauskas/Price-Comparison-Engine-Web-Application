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
using Microsoft.AspNetCore.Mvc.Formatters;
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
            ViewBag.Text = "Siųsti";
            Console.WriteLine("kvietimas");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(InputModel input)
        {
            ViewBag.Text = "Siųsti";
            if (input.EmailModel != null)
            {
                Console.WriteLine("1");
                if (ModelState.IsValid)
                {
                    Console.WriteLine("1 pavyko");
                    var confirmCode = GenerateHash.CreateSalt(16);
                    confirmCode = confirmCode.Remove(confirmCode.Length - 2);
                    var email = input.EmailModel.Email;

                    EmailSender.SendEmail(confirmCode, "ernestas20111@gmail.com");

                    TempData["tempEmail"] = email;
                    TempData["tempCode"] = confirmCode;
                    ViewBag.Text = "Patvirtinti";
                    //ViewBag.DisabledValue = 1;
                    //disable email
                    //enable code
                    //ViewBag.EmailDisabled = true;
                    //ViewBag.CodeEnabled = true;
                }
            }
            else if (input.CodeModel != null)
            {
                Console.WriteLine("2");
                var confirmCode = TempData["tempCode"].ToString();
                TempData["tempCode"] = confirmCode;
                Console.WriteLine(confirmCode);
                Console.WriteLine(input.CodeModel.Code);

                if (ModelState.IsValid)
                {
                    Console.WriteLine("2 pavyko");
                    if (input.CodeModel.Code.Equals(confirmCode))
                    {
                        Console.WriteLine("2 maybe");
                        //ViewBag.DisabledValue = 2;
                        //disable code
                        //enable password
                        //ViewBag.CodeEnabled = false;
                        //ViewBag.PasswordEnabled = true;
                        ViewBag.Text = "Pakeisti";
                    }
                    else
                    {
                        Console.WriteLine("2 nlb");
                        ViewBag.ShowMessage = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("3");
                if (ModelState.IsValid)
                {
                    var email = TempData["tempEmail"].ToString();
                    //TempData["tempEmail"] = email;
                    //_databaseManager.ChangePassword(email, input.PasswordModel.Password, input.PasswordModel.ConfirmPassword);
                    Console.WriteLine(email);
                    Console.WriteLine("3 pavyko");
                    return RedirectToAction("Login", "Logging");
                }
            }
            Console.WriteLine("fail");
            return View();
        }
    }
}