using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class AdministrationController : Controller
    {
        public IActionResult Admin()
        {
            var users = new List<User>();
            var DBmanager = new DatabaseManager();
            users = DBmanager.ReadUsersList();
            var AdminView = new AdminView() {Users = users};

            return View(AdminView);
        }

        public IActionResult Add(string email, string password)
        {
            var DBmanager = new DatabaseManager();
            var users = DBmanager.ReadUsersList();
            foreach(var user in users)
            {
                if(user.Email == email)
                {
                    return View();
                }
            }
            var newUser = new User(){ Email = email, Role = Role.User};
            DBmanager.CreateAccount(email, password);
            users.Add(newUser);
            var adminView = new AdminView() { Users = users };
            return View(adminView);

        }

        public IActionResult Remove(string email)
        {
            var DBmanager = new DatabaseManager();
            var users = DBmanager.ReadUsersList();
            var temp = users.FindAll(x => x.Email == email);
            if (temp.Count > 0)
            {
                DBmanager.DeleteAccount(email);
                var adminView = new AdminView() { Users = users.Except(temp).ToList() };
                return View(adminView);
            }

            return View();

        }
    }
}
