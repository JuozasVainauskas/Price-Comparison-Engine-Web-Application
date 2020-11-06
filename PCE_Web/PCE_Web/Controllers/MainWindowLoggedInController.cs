using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class MainWindowLoggedInController : Controller
    {
        public static string emaill = "";
        public IActionResult Items(string email)
        {
            emaill = email;

            if (DatabaseManager.ReadSlidesList().Any())
                {
                        var products = DatabaseManager.ReadSlidesList();
                        var productsSaved = DatabaseManager.ReadSavedItems(emaill);
                        var slideshowView = new SlideshowView
                        {
                            Products = products,
                            ProductsSaved = productsSaved
                        };

                        return View(slideshowView);
                }
                else
                {
                        var products = new List<Slide>();
                        var productsSaved = DatabaseManager.ReadSavedItems(emaill);
                        var notExistingItem = new Slide
                        {
                            PageUrl = "", ImgUrl = "~/img/suggestions/1.jpg"
                        };
                        products.Add(notExistingItem);

                        var slideshowView = new SlideshowView
                        {
                            Products = products,
                            ProductsSaved = productsSaved
                        };
                        return View(slideshowView);
                }
        }
    }
}
