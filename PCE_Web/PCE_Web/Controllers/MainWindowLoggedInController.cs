using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class MainWindowLoggedInController : Controller
    {
        public IActionResult Items()
        {
            if (DatabaseManager.ReadSearchedItems().Any())
            {
                var products = new List<Item>();
                foreach (var item in DatabaseManager.ReadSearchedItems())
                {
                    products.Add(item);
                }
                var slideshowView = new SlideshowView
                {
                    Products = products
                };
                return View(slideshowView);
            }
            else
            {
                var products = new List<Item>();
                var notExistingItem = new Item { Link = "", Picture = "~/img/suggestions/1.jpg", Seller = "", Name = "", Price = "" };
                products.Add(notExistingItem);

                var slideshowView = new SlideshowView
                {
                    Products = products
                };
                return View(slideshowView);
            }
        }
    }
}
