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
        private readonly IEmailSenderInterface _emailSender;

        public ReportsController(IDatabaseManager databaseManager, IEmailSenderInterface emailSender)
        {
            _databaseManager = databaseManager;
            _emailSender = emailSender;
        }

        public IActionResult Report(string email = "")
        {
            var comments = _databaseManager.ReadReports(email);
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var reportView = new ReportView() { Comments = comments, Role = role };
            return View(reportView);
        }

        public IActionResult Answer(string email, string answer)
        {
            _emailSender.AnswerReportMessage(email, 1, answer);
            return RedirectToAction("Report", "Reports", new { Email = email });
        }
    }
}
