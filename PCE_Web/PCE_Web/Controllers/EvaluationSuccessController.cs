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
        private readonly Lazy<int[]> _allowedShopId = new Lazy<int[]>(() => new[] { 1, 2, 3, 4, 5, 6, 7 });
        private readonly Lazy<int[]> _allowedRate = new Lazy<int[]>(() => new[] { 1, 2, 3, 4, 5});
        private readonly ICommentsManager _commentsManager;

        public EvaluationSuccessController(ICommentsManager commentsManager)
        {
            _commentsManager = commentsManager;
        }

        public IActionResult Success(int shopId, int rate,string comment)
        {
            var currentEmail = User.Identity.Name;

            if (!_commentsManager.IsAlreadyCommented(currentEmail,shopId) && currentEmail!=null && _allowedShopId.Value.Contains(shopId) && _allowedRate.Value.Contains(rate))
            {
                _commentsManager.WriteComments(currentEmail, shopId, rate, comment);
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
