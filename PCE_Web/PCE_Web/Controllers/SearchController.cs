using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductsCache _productsCache;
        public SearchController( IProductsCache productsCache)
        {
            _productsCache = productsCache;
        }

        [AllowAnonymous]
        public ActionResult Suggestions(string productName, int page)
        {
            var cachedItems = _productsCache.GetCachedItems(productName);
            if (cachedItems!=null)
            {
                var products = new List<Item>();
                foreach (var cachedItem in cachedItems)
                {
                    products.Add(cachedItem);
                }
                var suggestionsView = new SuggestionsView { Products = products };
                return View(suggestionsView);
            }
            else
            {
                var products = GetProductsFromApi.GetProducts(productName);
                _productsCache.SetCachedItems(productName,products);
                var productsToShow = products.Take(10).ToList();
                var suggestionsView = new SuggestionsView {Products = products};
                return View(suggestionsView);

            }
        }
    }
}
