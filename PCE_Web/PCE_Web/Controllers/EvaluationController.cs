using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PCE_Web.Controllers
{
    public class EvaluationController : Controller
    {
        public IActionResult Evaluate()
        {
            return View();
        }
    }
}
