using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PCE_Web.Controllers
{
    public class LoggingController : Controller
    {
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Turite įrašyti email.")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
