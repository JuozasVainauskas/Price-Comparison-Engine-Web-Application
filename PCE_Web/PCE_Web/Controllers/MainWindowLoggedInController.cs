using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class MainWindowLoggedInController : Controller
    {
        public IActionResult Items()
        {
                var products = new List<Item>();

                using (var context = new PCEDatabaseContext())
                {
                    var result = context.ItemsTable.Select(x => new Item { Link = x.PageUrl, Picture = x.ImgUrl, Seller = x.ShopName, Name = x.ItemName, Price = x.Price }).ToList();

                    foreach (var product in result)
                    {
                        products.Add(product);
                    }
                }
                return View();
        }

    }
}
