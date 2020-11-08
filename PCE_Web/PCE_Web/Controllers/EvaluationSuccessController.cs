using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;

namespace PCE_Web.Controllers
{
    public class EvaluationSuccessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Success(string email, int shopId, int rating, string comment)
        {
            if (!DatabaseManager.IsAlreadyCommented(email,shopId) && email!=null && comment!= null)
            {
                DatabaseManager.WriteComments(email, shopId, rating, comment);
               // MainWindowLoggedInController.emailCurrentUser
            }
            return View();
        }
    }
}
