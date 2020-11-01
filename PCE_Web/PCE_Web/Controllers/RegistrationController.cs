using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PCE_Web.Controllers
{
    public class RegistrationController : Controller
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Turite įrašyti email.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "Slaptažodis turi būti bet 4 simbolių ilgio.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Turite patvirtinti slaptažodį.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Slaptažodis turi sutapti su patvirtinimo slaptažodžiu.")]
            public string ConfirmPassword { get; set; }
        }


        public IActionResult Register()
        {
            return View();
        }

    }
}
