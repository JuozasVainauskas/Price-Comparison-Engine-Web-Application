using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Controllers
{
    public class ConfirmPasswordController : Controller
    {
        [HttpPost]
        public IActionResult EmailConfirmation()
        {
            return View();
        }
    }
}
