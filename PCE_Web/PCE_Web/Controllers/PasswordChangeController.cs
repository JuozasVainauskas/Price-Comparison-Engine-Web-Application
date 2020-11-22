﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly IDatabaseManager _databaseManager;

        private readonly EmailSenderInterface _emailsender;

        public PasswordChangeController(IDatabaseManager databaseManager, EmailSenderInterface emailSender)
        {
            _databaseManager = databaseManager;
            _emailsender = emailSender;
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

                    _emailsender.SendEmail(confirmCode, "ernestas20111@gmail.com");

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
                    TempData["AlertBox"] = "Sėkmingai pakeitėte slaptažodį";

                    return RedirectToAction("Login", "Logging");
                }
            }
            return View();
        }
    }
}