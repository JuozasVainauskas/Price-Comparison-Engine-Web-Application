﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class SearchLoggedInController : Controller
    {
        public static int IsSaved;
        public static string SearchWord = "";
        public static int SoldOutBarbora;
        public static int SoldOut;
        public delegate void WriteData<THtmlNode, TItem>(List<THtmlNode> productListItems, List<TItem> products);
        public delegate List<HtmlNode> Search<in THtmlDocument>(THtmlDocument htmlDocument);
        private readonly IHttpClientFactory _httpClient;
        private readonly ISavedItemsManager _savedItemsManager;
        private readonly IProductsCache _productsCache;
        private readonly IExceptionsManager _exceptionsManager;
        private delegate void SavingAnItemD(string link, string pictureUrl, string seller, string name, string price);

        public SearchLoggedInController(IHttpClientFactory httpClient, ISavedItemsManager savedItemsManager, IProductsCache productsCache, IExceptionsManager exceptionsManager)
        {
            _httpClient = httpClient;
            _savedItemsManager = savedItemsManager;
            _productsCache = productsCache;
            _exceptionsManager = exceptionsManager;
        }
        
        public async Task<IActionResult> Suggestions(string productName, string link, string pictureUrl, string seller, string name, string price, int page)
        {
            SavingAnItemD savingAnItem = null;
            savingAnItem += SavingAnItem;
            if (productName != null)
            {
                SearchWord = productName;
            }
            var cachedItems = _productsCache.GetCachedItems(SearchWord);
            if (cachedItems != null)
            {
                if (link != null)
                {
                    savingAnItem(link, pictureUrl, seller, name, price);
                    IsSaved = 1;
                    SuggestionsView.AlertBoxText = "Prekė sėkmingai išsaugota!";
                }
                if (IsSaved == 0)
                {
                    SuggestionsView.AlertBoxText = "Pasirinkite prekę, kurią norite išsaugoti arba naršykite toliau!";
                }
                var products = new List<Item>();
                foreach(var cachedItem in cachedItems)
                {
                    products.Add(cachedItem);
                }

                if (page > 1)
                {
                    var productsToShow = products.Skip(page * 10).Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, Page = page, ProductName = productName };
                    IsSaved = 0;
                    return View(suggestionsView);
                }
                else if (page == 1)
                {
                    var productsToShow = products.Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, Page = page, ProductName = productName };
                    IsSaved = 0;
                    return View(suggestionsView);
                }
                else
                {
                    var suggestionsView = new SuggestionsView { Products = products, Page = 1, ProductName = productName };
                    IsSaved = 0;
                    return View(suggestionsView);
                }

            }
            else
            {
                SuggestionsView.AlertBoxText = "Pasirinkite prekę, kurią norite išsaugoti arba naršykite toliau!";
                if (link != null)
                {
                    savingAnItem(link, pictureUrl, seller, name, price);
                }
                var httpClient = _httpClient.CreateClient();
                var products = await FetchAlgorithm.FetchAlgorithmaAsync(SearchWord, httpClient, _exceptionsManager);
                _productsCache.SetCachedItems(SearchWord, products);
                if (page > 1)
                {
                    var productsToShow = products.Skip(page * 10).Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else if (page == 1)
                {
                    var productsToShow = products.Take(10).ToList();
                    var suggestionsView = new SuggestionsView { Products = productsToShow, Page = page, ProductName = productName };
                    return View(suggestionsView);
                }
                else
                {
                    var suggestionsView = new SuggestionsView { Products = products, Page = 1, ProductName = productName };
                    return View(suggestionsView);
                }
            }
        }

        public void SavingAnItem(string link, string pictureUrl, string seller, string name, string price)
        {
            _savedItemsManager.WriteSavedItem(link, pictureUrl, seller, name, price, User.Identity.Name);
        }
    }
}
       
