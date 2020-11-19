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

        [AllowAnonymous]
        public IActionResult ChangePassword()
        {
            ViewBag.Text = "Siųsti";
            ViewBag.EmailEnabled = true;
            ViewBag.CodeEnabled = false;
            ViewBag.Password = false;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(InputModel input)
        {
            ViewBag.Text = "Siųsti";
            ViewBag.EmailEnabled = true;
            ViewBag.CodeEnabled = false;
            ViewBag.Password = false;

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
                    ViewBag.Text = "Patvirtinti";
                    ViewBag.Email = email;
                    ViewBag.EmailEnabled = false;
                    ViewBag.CodeEnabled = true;
                }
            }
            else if (input.CodeModel != null)
            {
                var confirmCode = TempData["tempCode"].ToString();
                var email = TempData["tempEmail"].ToString();
                TempData["tempCode"] = confirmCode;
                TempData["tempEmail"] = email;

                ViewBag.Text = "Patvirtinti";
                ViewBag.Email = email;
                ViewBag.EmailEnabled = false;
                ViewBag.CodeEnabled = true;

                if (ModelState.IsValid)
                {
                    if (input.CodeModel.Code.Equals(confirmCode))
                    {
                        ViewBag.Text = "Pakeisti";
                        ViewBag.Code = input.CodeModel.Code;
                        ViewBag.EmailEnabled = false;
                        ViewBag.CodeEnabled = false;
                        ViewBag.PasswordEnabled = true;
                    }
                    else
                    {
                        ViewBag.ShowMessage = true;
                    }
                }
            }
            else if (input.PasswordModel != null)
            {
                var confirmCode = TempData["tempCode"].ToString();
                var email = TempData["tempEmail"].ToString();
                TempData["tempCode"] = confirmCode;
                TempData["tempEmail"] = email;

                ViewBag.Text = "Pakeisti";
                ViewBag.Email = email;
                ViewBag.Code = confirmCode;
                ViewBag.EmailEnabled = false;
                ViewBag.CodeEnabled = false;
                ViewBag.PasswordEnabled = true;

                if (ModelState.IsValid)
                {
                    _databaseManager.ChangePassword(email, input.PasswordModel.Password, input.PasswordModel.ConfirmPassword);
                    return RedirectToAction("Login", "Logging");
                }
            }
            return View();
        }
    }
}