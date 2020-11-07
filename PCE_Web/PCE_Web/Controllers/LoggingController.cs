using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class LoggingController : Controller
    {
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
        public IActionResult Login(InputModel input)
        {
            if (ModelState.IsValid)
            {
                var user = DatabaseManager.LoginUser(input.Email, input.Password);
                if (user != null)
                {
                    return RedirectToAction("Items", "MainWindowLoggedIn", new {email = input.Email});
                }
                else
                {
                    ViewBag.ShowMessage = true;
                }
            }

            return View();
        }
    }
}
