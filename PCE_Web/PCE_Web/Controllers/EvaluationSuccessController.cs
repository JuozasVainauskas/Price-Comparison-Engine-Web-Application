using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class EvaluationSuccessController : Controller
    {

        private readonly int[] allowedShopId = { 1, 2, 3, 4, 5, 6, 7 };
        private readonly int[] allowedRate = { 1, 2, 3, 4, 5 };
        private readonly IDatabaseManager _databaseManager;

        public EvaluationSuccessController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public IActionResult Success(int shopId, int rate,string comment)
        {
            
            var currentEmail = MainWindowLoggedInController.EmailCurrentUser;

            if (!_databaseManager.IsAlreadyCommented(currentEmail,shopId) && currentEmail!=null && allowedShopId.Contains(shopId) && allowedRate.Contains(rate))
            {
                _databaseManager.WriteComments(currentEmail, shopId, rate, comment);
                return View();
            }
            else
            {
                return RedirectToAction("Failure", "EvaluationSuccess");
            }
            
        }

        public IActionResult Failure()
        {
            return View();
        }
    }
}
