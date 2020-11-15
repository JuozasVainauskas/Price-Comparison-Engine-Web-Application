using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class ExceptionHandlingController : Controller
    {
        private readonly IDatabaseManager _databaseManager;
        public ExceptionHandlingController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        [Route("Error")]
        public IActionResult Error(Exception exception)
        {
            _databaseManager.WriteLoggedException(exception.InnerException.GetType().ToString());
            //exception.InnerException.GetType().ToString()
            return View("Error");
        }
        
    }
}
