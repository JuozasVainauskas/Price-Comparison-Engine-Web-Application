using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class MainWindowLoggedInController : Controller
    {
        public IActionResult Items()
        {
            if (DatabaseManager.ReadSlidesList().Any())
            {
                var products = DatabaseManager.ReadSlidesList();
                var slideshowView = new SlideshowView
                {
                    Products = products
                };
                return View(slideshowView);
            }
            else
            {
                var products = new List<Slide>();
                var notExistingItem = new Slide
                {
                    PageUrl = "", ImgUrl = "~/img/suggestions/1.jpg"
                };
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
