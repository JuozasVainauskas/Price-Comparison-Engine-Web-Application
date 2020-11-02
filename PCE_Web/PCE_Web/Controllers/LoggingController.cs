using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class LoggingController : Controller
    {
        [BindProperty]
        public InputModel Input { get; set; }
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn()
        {
            if (ModelState.IsValid)
            {
                var user = DbMngClass.LoginUser(Input.Email, Input.Password);
                if (user != null)
                {
                    return View("~/Views/MainWindowLoggedIn/Items.cshtml");
                }
                else
                {
                    ViewBag.ShowMessage = true;
                }
            }

            return View("~/Views/Logging/Login.cshtml");
        }
    }
}
