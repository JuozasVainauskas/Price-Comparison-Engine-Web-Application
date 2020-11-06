using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class MainWindowLoggedInController : Controller
    {
        public static string emaill = "";
        public IActionResult Items(string email, string link, string pictureUrl, string seller, string name, string price)
        {
            if (email != null)
            {
                emaill = email;
            }

            if (link != null)
            {
                var productToDelete = new Item
                {
                    Link = link,
                    Picture = pictureUrl,
                    Seller = seller,
                    Name = name,
                    Price = price
                };
                DatabaseManager.DeleteSavedItem(emaill, productToDelete);
            }
            if (DatabaseManager.ReadSlidesList().Any())
            {
                var products = DatabaseManager.ReadSlidesList();
                var productsSaved = DatabaseManager.ReadSavedItems(emaill);
                var slideshowView = new SlideshowView
                {
                    ProductsSaved = productsSaved,
                    Products = products
                };

                return View(slideshowView);
            }
            else
            {
                var products = new List<Slide>();
                var productsSaved = DatabaseManager.ReadSavedItems(emaill);
                var notExistingItem = new Slide
                {
                    PageUrl = "",
                    ImgUrl = "~/img/suggestions/1.jpg"
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
