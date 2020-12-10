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
        public static readonly Dictionary<int, string> _allowedShopId = new Dictionary<int, string> { { 1, "Avitela" }, { 2, "Gintarine" }, { 3, "Barbora" }, { 4, "RDE" }, { 5, "BigBox" }, { 6, "Elektromarkt" }, { 7, "Pigu" } };
        private readonly Lazy<int[]> _allowedRate = new Lazy<int[]>(() => new[] { 1, 2, 3, 4, 5});
        private readonly ICommentsManager _commentsManager;

        public EvaluationSuccessController(ICommentsManager commentsManager)
        {
            _commentsManager = commentsManager;
        }

        public IActionResult Success(int shopId, int rate,string comment)
        {
            var currentEmail = User.Identity.Name;

            if (!_commentsManager.IsAlreadyCommented(currentEmail,shopId) && currentEmail!=null && _allowedShopId.ContainsKey(shopId) && _allowedRate.Value.Contains(rate))
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
