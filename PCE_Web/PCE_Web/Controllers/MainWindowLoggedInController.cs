using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class MainWindowLoggedInController : Controller
    {
        public static int IsDeletedOrSaved = 1;
        private readonly IReportsManager _reportsManager;
        private readonly ISavedItemsManager _savedItemsManager;
        private readonly IEmailSender _emailSender;
        private readonly IProductsCache _productsCache;
        private readonly string[] _shops={"Avitela", "Gintarinė", "Barbora", "Rde", "BigBox", "Elektromarkt", "Pigu"};

        public MainWindowLoggedInController(IEmailSender emailSender, IProductsCache productsCache, IReportsManager reportsManager, ISavedItemsManager savedItemsManager)
        {
            _emailSender = emailSender;
            _productsCache = productsCache;
            _reportsManager = reportsManager;
            _savedItemsManager = savedItemsManager;
        }

        public IActionResult Items(string link, string pictureUrl, string seller, string name, string price)
        {
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
                _savedItemsManager.DeleteSavedItem(User.Identity.Name, productToDelete);
                IsDeletedOrSaved = 0;
            }

            if (IsDeletedOrSaved == 1)
            {
                SlideshowView.AlertBoxText = "Sėkmingai prisijungėte!";
            }
            else if (IsDeletedOrSaved == 0)
            {
                SlideshowView.AlertBoxText = "Sėkmingai ištrinta prekė!";
            }
            else
            {
                SlideshowView.AlertBoxText = "Sveiki sugrįžę!";
            }
            var cachedItems = _productsCache.GetCachedItems();
            if (cachedItems!=null)
            {
                var products = new List<Slide>();
                foreach (var cachedItem in cachedItems)
                {
                    foreach(var cachedItema in cachedItem)
                    {
                        products.Add(new Slide() { ImgUrl = cachedItema.Picture, PageUrl = cachedItema.Link });
                    }
                    
                }

                var productsSaved = _savedItemsManager.ReadSavedItems(User.Identity.Name);
               
                var slideshowView = new SlideshowView
                {
                    ProductsSaved = productsSaved,
                    Products = products,
                    Shops = _shops
                };

                IsDeletedOrSaved = 2;
                return View(slideshowView);
            }
            else
            {
                var products = new List<Slide>();
                var productsSaved = _savedItemsManager.ReadSavedItems(User.Identity.Name);
                var notExistingItem = new Slide
                {
                    PageUrl = "",
                    ImgUrl = "~/img/suggestions/1.jpg"
                };
                products.Add(notExistingItem);

                var slideshowView = new SlideshowView
                {
                    Products = products,
                    ProductsSaved = productsSaved,
                    Shops = _shops
                };
                IsDeletedOrSaved = 2;
                return View(slideshowView);
            }
        }

        public IActionResult Report(string report)
        {
            _reportsManager.WriteReports(User.Identity.Name,report);
            _emailSender.AnswerReportMessage(User.Identity.Name, 0);
            return RedirectToAction("Items", "MainWindowLoggedIn");
        }
    }
}
