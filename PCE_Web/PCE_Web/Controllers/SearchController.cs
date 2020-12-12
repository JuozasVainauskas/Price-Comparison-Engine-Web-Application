using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
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

                int maxPage = products.Count / 10;
                if (products.Count % 10 != 0)
                {
                    ++maxPage;
                }

                if (page > 1 && page < maxPage)
                {
                    var productsToShow = products.Skip((page - 1) * 10).Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page == 1 && page < maxPage)
                {
                    var productsToShow = products.Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page == 1 && page == maxPage)
                {
                    var suggestionsView = new SuggestionsView { Products = products, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page > 1 && page == maxPage)
                {
                    var productsToShow = products.Skip((page - 1) * 10).Take(products.Count - ((page - 1) * 10)).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else
                {
                    var productsToShow = products.Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = 1, ProductName = productName };
                    return View(suggestionsView);
                }
            }
            else
            {
                var products = GetProductsFromAPI(productName);
                _productsCache.SetCachedItems(productName,products);

                int maxPage = products.Count / 10;
                if (products.Count % 10 != 0)
                {
                    ++maxPage;
                }

                if (page > 1 && page < maxPage)
                {
                    var productsToShow = products.Skip((page - 1) * 10).Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page == 1 && page < maxPage)
                {
                    var productsToShow = products.Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page == 1 && page == maxPage)
                {
                    var suggestionsView = new SuggestionsView { Products = products, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page > 1 && page == maxPage)
                {
                    var productsToShow = products.Skip((page - 1) * 10).Take(products.Count - ((page - 1) * 10)).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else
                {
                    var productsToShow = products.Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, AllProducts = products, Page = 1, ProductName = productName };
                    return View(suggestionsView);
                }
            }
        }
        //iskelti api
        private List<Item> GetProductsFromAPI(string productName)
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(clientHandler);
            try
            {
                var resultList = new List<Item>();
                var getDataTask = client.GetAsync("https://localhost:44320/api/Products/" + productName).ContinueWith(
                    response =>
                    {
                        var result = response.Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var readResult = result.Content.ReadAsAsync<List<Item>>();
                            readResult.Wait();
                            resultList = readResult.Result;
                        }
                    });
                getDataTask.Wait();
                return resultList;
            }
            catch (Exception)
            {
                Console.WriteLine();
                throw;
            }
        }
    }
}
