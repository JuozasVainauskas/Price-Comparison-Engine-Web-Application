using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class RegistrationController : Controller
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Email")]
            [Required(ErrorMessage = "Turite įrašyti email.")]
            [EmailSpelling]
            [EmailExistence]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "Slaptažodis turi būti bet 4 simbolių ilgio.")]
            [PasswordSpelling]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Required(ErrorMessage = "Turite patvirtinti slaptažodį.")]
            [Compare("Password", ErrorMessage = "Slaptažodis turi sutapti su patvirtinimo slaptažodžiu.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp()
        {
            if (ModelState.IsValid)
            {
                DbMngClass.RegisterUser(Input.Email, Input.Password);
                return View("~/Views/MainWindowLoggedIn/Items.cshtml");
            }

            return View("~/Views/Registration/Register.cshtml");
        }
    }
}
