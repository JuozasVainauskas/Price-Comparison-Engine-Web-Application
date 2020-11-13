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
    }
}
