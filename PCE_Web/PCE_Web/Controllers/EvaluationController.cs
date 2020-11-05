using Microsoft.AspNetCore.Mvc;

namespace PCE_Web.Controllers
{
    public class EvaluationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Evaluate()
        {
            return View();
        }
    }
}
