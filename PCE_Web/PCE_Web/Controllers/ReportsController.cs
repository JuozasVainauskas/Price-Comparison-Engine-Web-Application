using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IDatabaseManager _databaseManager;
        public ReportsController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public IActionResult Report(string email = "")
        {
            List<string> comments = _databaseManager.ReadReport(email);
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var reportView = new ReportView() { comments = comments, Role = role };
            return View(reportView);
        }
    }
}
