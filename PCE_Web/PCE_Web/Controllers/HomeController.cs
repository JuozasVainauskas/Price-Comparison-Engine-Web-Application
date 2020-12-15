using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IExceptionsManager _exceptionsManager;

        public HomeController(IExceptionsManager exceptionsManager)
        {
            _exceptionsManager = exceptionsManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }
      
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            _exceptionsManager.WriteLoggedExceptions(feature.Error.Message, feature.Error.Source, feature.Error.StackTrace, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
