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
        public static int isDeletedOrSaved = 1;
        public static string EmailCurrentUser = "";

        public IActionResult Items(string email, string link, string pictureUrl, string seller, string name, string price)
        {
        
            if (email != null)
            {
                EmailCurrentUser = email;
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
                DatabaseManager.DeleteSavedItem(EmailCurrentUser, productToDelete);
                isDeletedOrSaved = 0;
            }

            if (isDeletedOrSaved == 1)
            {
                SlideshowView.AlertBoxText = "Sėkmingai prisijungėte!";
            }
            else if (isDeletedOrSaved == 0)
            {
                SlideshowView.AlertBoxText = "Sėkmingai ištrinta prekė!";
            }
            else
            {
                SlideshowView.AlertBoxText = "Sveiki sugrįžę!";
            }

            if (DatabaseManager.ReadSlidesList().Any())
            {
                var products = DatabaseManager.ReadSlidesList();
                var productsSaved = DatabaseManager.ReadSavedItems(EmailCurrentUser);
               
                var slideshowView = new SlideshowView
                {
                    ProductsSaved = productsSaved,
                    Products = products
                };

                isDeletedOrSaved = 2;
                return View(slideshowView);
            }
            else
            {
                var products = new List<Slide>();
                var productsSaved = DatabaseManager.ReadSavedItems(EmailCurrentUser);
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
                isDeletedOrSaved = 2;
                return View(slideshowView);
            }
  
        }
    }
}
