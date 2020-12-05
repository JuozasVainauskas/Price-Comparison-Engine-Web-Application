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
        private readonly IDatabaseManager _databaseManager;

        public SearchController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [AllowAnonymous]
        public ActionResult Suggestions(string productName)
        {
            if (_databaseManager.ReadSearchedItems(productName).Any())
            {
                var products = new List<Item>();
                foreach (var item in _databaseManager.ReadSearchedItems(productName))
                {
                    products.Add(item);
                }
                var suggestionsView = new SuggestionsView {Products = products};
                return View(suggestionsView);
            }
            else
            {
                var products = GetProductsFromAPI(productName);
                _databaseManager.WriteSearchedItems(products, productName);
                var suggestionsView = new SuggestionsView {Products = products};
                return View(suggestionsView);
            }
        }

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
