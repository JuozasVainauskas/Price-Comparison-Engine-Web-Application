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
        private readonly IHttpClientFactory _httpClient;
        private readonly IExceptionsManager _exceptionsManager;

        public SearchSpecificationsController(IHttpClientFactory httpClient, IExceptionsManager exceptionsManager)
        {
            _httpClient = httpClient;
            _exceptionsManager = exceptionsManager;
        }

        public async Task<IActionResult> SuggestionsSpecifications(string productName, int lowestPrice, int biggestPrice, string[] tags)
        {
            var httpClient = _httpClient.CreateClient();
            var products = await FetchAlgorithmForSpecifications.FetchAlgorithmaSpecfications(productName, httpClient, lowestPrice*100, biggestPrice*100, tags, _exceptionsManager);
            var suggestionsSpecificationsView = new SuggestionsSpecificationsView { Products = products };
            return View(suggestionsSpecificationsView);
        }

    }
}
