﻿using System;
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
        private readonly IDatabaseManager _databaseManager;
        private delegate void SavingAnItemD(string link, string pictureUrl, string seller, string name, string price);

        public SearchLoggedInController(IHttpClientFactory httpClient, IDatabaseManager databaseManager)
        {
            _httpClient = httpClient;
            _databaseManager = databaseManager;
        }

        public async Task<IActionResult> Suggestions(string productName, string link, string pictureUrl, string seller, string name, string price)
        {
            SavingAnItemD savingAnItem = null;
            savingAnItem += SavingAnItem;
            if (productName != null)
            {
                SearchWord = productName;
            }
            if (_databaseManager.ReadSearchedItems(SearchWord).Any())
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
                foreach (var item in _databaseManager.ReadSearchedItems(SearchWord))
                {
                    products.Add(item);
                }
                var suggestionsView = new SuggestionsView { Products = products };
                IsSaved = 0;
                return View(suggestionsView);
            }
            else
            {
                SuggestionsView.AlertBoxText = "Pasirinkite prekę, kurią norite išsaugoti arba naršykite toliau!";
                if (link != null)
                {
                    savingAnItem(link, pictureUrl, seller, name, price);
                }
                var httpClient = _httpClient.CreateClient();
                // var products = new List<Item>();
                // await ReadingItemsAsync(SearchWord, products, httpClient);
                // products = SortAndInsert(products);
                var products = await FetchAlgorithm.FetchAlgorithmaAsync(SearchWord, httpClient, _databaseManager);
                _databaseManager.WriteSearchedItems(products, SearchWord);
                var suggestionsView = new SuggestionsView { Products = products };
                return View(suggestionsView);
            }
        }

        public void SavingAnItem(string link, string pictureUrl, string seller, string name, string price)
        {
            _databaseManager.WriteSavedItem(link, pictureUrl, seller, name, price, User.Identity.Name);
        }
    }
}
       
