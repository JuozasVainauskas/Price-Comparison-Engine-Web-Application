using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class SearchSpecificationsController : Controller
    {
        public static int SoldOutBarbora;
        public static int SoldOut;
        public delegate void WriteData<THtmlNode, TItem, in TInt>(List<THtmlNode> productListItems, List<TItem> products, TInt minPrice, TInt maxPrice);
        public delegate List<HtmlNode> Search<in THtmlDocument>(THtmlDocument htmlDocument);
        private readonly IHttpClientFactory _httpClient;
        private readonly IDatabaseManager _databaseManager;

        public SearchSpecificationsController(IHttpClientFactory httpClient, IDatabaseManager databaseManager)
        {
            _httpClient = httpClient;
            _databaseManager = databaseManager;
        }

        public async Task<IActionResult> SuggestionsSpecifications(string productName, int lowestPrice, int biggestPrice, string[] tags)
        {
            var httpClient = _httpClient.CreateClient();
            var products = await FetchAlgorithmForSpecifications.FetchAlgorithmaSpecfications(productName, httpClient, lowestPrice, biggestPrice, tags, _databaseManager);
            var suggestionsSpecificationsView = new SuggestionsSpecificationsView { Products = products };
            return View(suggestionsSpecificationsView);
        }

    }
}
