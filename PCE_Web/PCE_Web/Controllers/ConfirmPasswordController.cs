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
        public class InputModel
        {
            [Display(Name = "Email")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Display(Name = "ConfirmCode")]
            public string ConfirmCode { get; set; }
            [Display(Name = "InputCode")]
            [Compare("ConfirmCode", ErrorMessage = "Blogai įvestas kodas.")]
            public string InputCode { get; set; }
        }

        public IActionResult EmailConfirmation(string email, string password, string confirmCode)
        {
            InputModel inputModel = new InputModel()
            {
                Email = email,
                Password = password,
                ConfirmCode = confirmCode,
                InputCode = ""
            };
            return View(inputModel);
        }

        [HttpPost]
        public IActionResult EmailConfirmation(InputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                DatabaseManager.RegisterUser(inputModel.Email, inputModel.Password);
                return RedirectToAction("Items", "MainWindowLoggedIn", new { email = inputModel.Email });
            }
            return View();
        }
    }
}