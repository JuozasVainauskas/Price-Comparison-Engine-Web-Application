﻿using System;
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
    public class AdministrationController : Controller
    {
        private readonly IDatabaseManager _databaseManager;
        public AdministrationController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public IActionResult Admin(string messageString = "")
        {
            
            var exceptions = _databaseManager.ReadLoggedExceptions();
            ViewBag.MyMessage = messageString;
            var allUsers = _databaseManager.ReadUsersList();
            List<User> reportedUsers = new List<User>();

            foreach( var user in allUsers)
            {
                if(_databaseManager.isReported(user.Email))
                {
                    reportedUsers.Add(user);
                }
            }

            List<User> users = allUsers.Except(reportedUsers).ToList();
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var adminView = new AdminView() { Users = users, ReportedUsers = reportedUsers, Role = role, Exceptions = exceptions };
            
            return View(adminView);
        }

        public IActionResult Delete()
        {
                var exception = new Exceptions { Date = TempData["date"].ToString(), Message = TempData["message"].ToString(), Source = TempData["source"].ToString(), StackTrace = TempData["stacktrace"].ToString() };
                _databaseManager.DeleteLoggedExceptions(exception);

            return RedirectToAction("Admin", "Administration");
        }
            public IActionResult Add(string email, string password)
        {

            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                var users = _databaseManager.ReadUsersList();
                foreach (var user in users)
                {
                    if (user.Email == email)
                    {
                        return RedirectToAction("Admin", "Administration", new { messageString = "Toks vartotojas jau egzistuoja!" });
                    }
                }
                var newUser = new User() { Email = email, Role = Role.User };
                _databaseManager.CreateAccount(email, password);
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
                var users = _databaseManager.ReadUsersList();
                var temp = users.FindAll(x => x.Email == email);

                if (email != currentEmail)
                {
                    if (temp.Count > 0)
                    {
                        _databaseManager.DeleteAccount(email);
                        var adminView = new AdminView() { Users = users.Except(temp).ToList() };
                        return RedirectToAction("Admin", "Administration");
                    }
                    else
                    {
                        return RedirectToAction("Admin", "Administration", new { messageString = "Toks vartotojas neegzistuoja!" });
                    }
                }
                else
                {
                    return RedirectToAction("Admin", "Administration", new { messageString = "Savęs ištrinti negalite!" });
                }
            }
            return RedirectToAction("Admin", "Administration");
        }

        public IActionResult Set(string email, int roleID)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                _databaseManager.SetRole(email, roleID.ToString());
                return RedirectToAction("Admin", "Administration", new { messageString = "Rolė suteikta sėkmingai" });
            }
            else
            {
                return RedirectToAction("Admin", "Administration");
            }

        }
    }
}
