using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly IAccountManager _accountManager;
        private readonly IReportsManager _reportsManager;
        private readonly IExceptionsManager _exceptionsManager;

        public AdministrationController(IAccountManager accountManager, IReportsManager reportsManager, IExceptionsManager exceptionsManager)
        {
            _accountManager = accountManager;
            _reportsManager = reportsManager;
            _exceptionsManager = exceptionsManager;
        }

        public IActionResult Admin(string messageString = "")
        {
            
            var exceptions = _exceptionsManager.ReadLoggedExceptions();
            ViewBag.MyMessage = messageString;
            var allUsers = _accountManager.ReadUsersList();
            var reportedUsers = new List<User>();

            foreach( var user in allUsers)
            {
                if(_reportsManager.IsReported(user.Email))
                {
                    reportedUsers.Add(user);
                }
            }

            var users = allUsers.Except(reportedUsers).ToList();
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var adminView = new AdminView() { Users = users, ReportedUsers = reportedUsers, Role = role, Exceptions = exceptions };
            
            return View(adminView);
        }

        public IActionResult Delete(int id)
        {
            _exceptionsManager.DeleteLoggedExceptions(id);

            return RedirectToAction("Admin", "Administration");
        }

        public IActionResult Add(string email, string password)
        {

            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                var users = _accountManager.ReadUsersList();
                foreach (var user in users)
                {
                    if (user.Email == email)
                    {
                        return RedirectToAction("Admin", "Administration",
                            new { messageString = "Toks vartotojas jau egzistuoja!" });
                    }
                }
                var newUser = new User() { Email = email, Role = Role.User };
                _accountManager.CreateAccount(email, password);
                return RedirectToAction("Admin", "Administration");
            }
            else
            {
                return RedirectToAction("Admin", "Administration");
            }

        }

        public IActionResult Remove(string email)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var currentEmail = User.Identity.Name;
            if (role == "Admin")
            {
                var users = _accountManager.ReadUsersList();
                var temp = users.FindAll(x => x.Email == email);

                if (email != currentEmail)
                {
                    if (temp.Count > 0)
                    {
                        _accountManager.DeleteAccount(email);
                        var adminView = new AdminView() { Users = users.Except(temp).ToList() };
                        return RedirectToAction("Admin", "Administration");
                    }
                    else
                    {
                        return RedirectToAction("Admin", "Administration",
                            new { messageString = "Toks vartotojas neegzistuoja!" });
                    }
                }
                else
                {
                    return RedirectToAction("Admin", "Administration",
                        new { messageString = "Savęs ištrinti negalite!" });
                }
            }
            return RedirectToAction("Admin", "Administration");
        }

        public IActionResult Set(string email, int roleId)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                var temp = _accountManager.ReadUsersList().FindAll(x => x.Email == email);
                if (temp.Count > 0)
                {
                    _accountManager.SetRole(email, roleId.ToString());
                    return RedirectToAction("Admin", "Administration",
                        new { messageString = "Rolė suteikta sėkmingai" });
                }
                else
                {
                    return RedirectToAction("Admin", "Administration",
                        new { messageString = "Toks vartotojas neegzistuoja!" });
                }
            }
            else
            {
                return RedirectToAction("Admin", "Administration");
            }

        }
    }
}
