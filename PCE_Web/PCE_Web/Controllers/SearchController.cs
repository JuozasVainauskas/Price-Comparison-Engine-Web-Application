using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class SearchController : Controller
    {

        public class Item
        {
            public string Picture { get; set; }

            public string Seller { get; set; }

            public double Pricea { get; set; }

            public string Price { get; set; }

            public string Name { get; set; }

            public string Link { get; set; }
        }
        public IActionResult Suggestions(string ProductName)//Perduodamas produkto pavadinimas
        {
            var suggestionsView = new SuggestionsView();
            suggestionsView.message = "Artimiausiu metu čia galėsite pamatyti siūlomas prekes.";
            var prices = new List<Item>();
            return View(suggestionsView);
        }
    }
}
