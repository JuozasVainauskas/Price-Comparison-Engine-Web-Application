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
            ViewBag.MyMessage = messageString;
            var users = _databaseManager.ReadUsersList();
            var adminView = new AdminView() {Users = users};
            //Testavimo tikslais
            Console.WriteLine(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value);

            return View(adminView);
        }

         public IActionResult Add(string email, string password)
        {
            var users = _databaseManager.ReadUsersList();
            foreach(var user in users)
            {
                if(user.Email == email)
                {
                    return RedirectToAction("Admin", "Administration", new { messageString = "Toks vartotojas jau egzistuoja!" });
                }
            }
            var newUser = new User(){ Email = email, Role = Role.User};
            _databaseManager.CreateAccount(email, password);
            return RedirectToAction("Admin", "Administration");

        }

        public IActionResult Remove(string email)
        {
            var users = _databaseManager.ReadUsersList();
            var temp = users.FindAll(x => x.Email == email);
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

        public IActionResult Set(string email, int roleID)
        {
            _databaseManager.SetRole(email, roleID.ToString());
            return RedirectToAction("Admin", "Administration", new { messageString = "Rolė suteikta sėkmingai" });

        }
    }
}