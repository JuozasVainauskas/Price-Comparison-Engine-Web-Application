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
            var solvedComments = _databaseManager.ReadReports(email, 1);
            var unsolvedComments = _databaseManager.ReadReports(email, 0);
            var allComments = solvedComments.Concat(unsolvedComments).ToList();

            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var reportView = new ReportView() { AllComments = allComments, UnsolvedComments = unsolvedComments, SolvedComments = solvedComments, Role = role };
            return View(reportView);
        }

        public IActionResult Answer(string email, string answer)
        {
            _emailSender.AnswerReportMessage(email, 1, answer);
            return RedirectToAction("Report", "Reports", new { Email = email });
        }

        public IActionResult Delete (int id, string email)
        {
            _databaseManager.DeleteReports(id);

            if(_databaseManager.ReadReports(email, 0).Any() || _databaseManager.ReadReports(email, 1).Any())
            {
                return RedirectToAction("Report", "Reports", new { Email = email });
            }
            else
            {
                return RedirectToAction("Admin", "Administration");
            }
        }

        public IActionResult Mark (int id, string email)
        {
            _databaseManager.MarkAsSolved(id);
            return RedirectToAction("Report", "Reports", new { Email = email });
        }
    }
}
