using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class RegistrationController : Controller
    {
        public class InputModel
        {
            [Display(Name = "Email")]
            [DataType(DataType.EmailAddress)]
            [Required(ErrorMessage = "Turite įrašyti email.")]
            [EmailSpelling]
            [EmailExistence]
            public string Email { get; set; }

            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "Slaptažodis turi būti bet 4 simbolių ilgio.")]
            [PasswordSpelling]
            public string Password { get; set; }

            [Display(Name = "Confirm password")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Turite patvirtinti slaptažodį.")]
            [Compare("Password", ErrorMessage = "Slaptažodis turi sutapti su patvirtinimo slaptažodžiu.")]
            public string ConfirmPassword { get; set; }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(InputModel input)
        {
            if (ModelState.IsValid)
            {
                TempData["userEmail"] = input.Email;
                TempData["userPassword"] = input.Password;

                return RedirectToAction("EmailConfirmation", "ConfirmPassword");
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult ConfirmEmail()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Register", "Registration");
            }

            return RedirectToAction("Register", "Registration");
        }

        [AllowAnonymous]
        public IActionResult ChangePassword()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Register", "Registration");
            }

            return RedirectToAction("Register", "Registration");
        }
    }
}