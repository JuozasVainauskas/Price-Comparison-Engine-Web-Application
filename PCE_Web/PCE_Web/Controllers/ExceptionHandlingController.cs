using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PCE_Web.Controllers
{
    public class ExceptionHandlingController : Controller
    {

        [Route("Error")]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
