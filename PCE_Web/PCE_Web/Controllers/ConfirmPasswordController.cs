using System;
using System.ComponentModel.DataAnnotations;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class ConfirmPasswordController : Controller
    {
        private static string _email;
        private static string _password;
        private static string _confirmCode;

        public IActionResult EmailConfirmation(string email, string password)
        {
            var code = GenerateHash.CreateSalt(16);
            code = code.Remove(code.Length - 2);
            new SendEmail(code, "ernestas20111@gmail.com");

            _email = email;
            _password = password;
            _confirmCode = code;
            return View();
        }

        [HttpPost]
        public IActionResult EmailConfirmation(string inputCode)
        {
            if (inputCode!= null && inputCode.Equals(_confirmCode))
            {
                DatabaseManager.RegisterUser(_email, _password);
                return RedirectToAction("Items", "MainWindowLoggedIn", new { email = _email });
            }
            else
            {
                ViewBag.ShowMessage = true;
            }
            return View();
        }
    }
}