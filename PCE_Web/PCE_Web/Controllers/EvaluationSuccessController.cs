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

        public IActionResult Success(int shopId, int rate,string comment)
        {
            
            var currentEmail = MainWindowLoggedInController.EmailCurrentUser;

            if (!DatabaseManager.IsAlreadyCommented(currentEmail,shopId) && currentEmail!=null && comment!= null)
            {
                DatabaseManager.WriteComments(currentEmail, shopId, rate, comment);
            }
            
            Console.WriteLine(comment);
            Console.WriteLine(rate);
            Console.WriteLine(shopId);
            return View();
        }
    }
}
