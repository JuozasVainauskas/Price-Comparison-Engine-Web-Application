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
        public IActionResult Suggestions()
        {
            var suggestionsView= new SuggestionsView();
            suggestionsView.message = "Artimiausiu metu čia galėsite pamatyti siūlomas prekes.";
            return View(suggestionsView);
        }
    }
}
