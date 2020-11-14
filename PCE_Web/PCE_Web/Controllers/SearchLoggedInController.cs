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
    [Authorize]
    public class SearchLoggedInController : Controller
    {
        public static int IsSaved;
        public static string SearchWord  = "";
        public static int SoldOutBarbora;
        public static int SoldOut;
        public delegate void WriteData<THtmlNode, TItem>(List<THtmlNode> productListItems, List<TItem> products);
        public delegate List<HtmlNode> Search<in THtmlDocument>(THtmlDocument htmlDocument);
        private readonly IHttpClientFactory _httpClient;
        private readonly IDatabaseManager _databaseManager;

       
        public SearchLoggedInController(IHttpClientFactory httpClient, IDatabaseManager databaseManager)
        {
            _httpClient = httpClient;
            _databaseManager = databaseManager;
        }

        public async Task<IActionResult> Suggestions(string productName, string link, string pictureUrl, string seller, string name, string price)
        {
            throw new Exception("bad search");

            if (productName != null)
            {
                SearchWord = productName;
            }
            if (_databaseManager.ReadSearchedItems(SearchWord).Any())
            {
                if (link != null)
                {
                    _databaseManager.WriteSavedItem(link, pictureUrl, seller, name, price,User.Identity.Name);
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
                    _databaseManager.WriteSavedItem(link, pictureUrl, seller, name, price, User.Identity.Name);
                }
                var httpClient = _httpClient.CreateClient();
                var products = new List<Item>();
                await ReadingItemsAsync(SearchWord, products, httpClient);
                products = SortAndInsert(products);
                _databaseManager.WriteSearchedItems(products, SearchWord);
                var suggestionsView = new SuggestionsView { Products = products };
                return View(suggestionsView);
            }
        }
        private async Task ReadingItemsAsync(string productName, List<Item> products, HttpClient httpClient)
        {
            var gettingRde = await Task.Factory.StartNew(() => gettingItemsFromRde(productName, products, httpClient));
            var gettingBarbora = await Task.Factory.StartNew(() => gettingItemsFromBarbora(productName, products, httpClient));
            var gettingAvitela = await Task.Factory.StartNew(() => gettingItemsFromAvitela(productName, products, httpClient));
            var gettingPigu = await Task.Factory.StartNew(() => gettingItemsFromPigu(productName, products, httpClient));
            var gettingGintarine = await Task.Factory.StartNew(() => gettingItemsFromGintarineVaistine(productName, products, httpClient));
            var gettingElektromarkt = await Task.Factory.StartNew(() => gettingItemsFromElektromarkt(productName, products, httpClient));
            var gettingBigBox = await Task.Factory.StartNew(() => gettingItemsFromBigBox(productName, products, httpClient));
            var taskList = new List<Task>
            {
                gettingRde, gettingBarbora, gettingAvitela, gettingPigu, gettingGintarine, gettingElektromarkt,
                gettingBigBox
            };
            Task.WaitAll(taskList.ToArray());
        }
        private async Task gettingItemsFromRde(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlRde = "https://www.rde.lt/search_result/lt/word/" + productName + "/page/1";
            Search<HtmlDocument> rdeSearch = RdeSearch;
            WriteData<HtmlNode, Item> writeDataFromRde = WriteDataFromRde;
            var rdeItems = rdeSearch(await Html(httpClient, urlRde));
            writeDataFromRde(rdeItems, products);
        }

        private async Task gettingItemsFromBarbora(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlBarbora = "https://pagrindinis.barbora.lt/paieska?q=" + productName;
            Search<HtmlDocument> barboraSearch = BarboraSearch;
            WriteData<HtmlNode, Item> writeDataFromBarbora = WriteDataFromBarbora;
            var barboraItems = barboraSearch(await Html(httpClient, urlBarbora));
            writeDataFromBarbora(barboraItems, products);
        }

        private async Task gettingItemsFromAvitela(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlAvitela = "https://avitela.lt/paieska/" + productName;
            Search<HtmlDocument> avitelaSearch = AvitelaSearch;
            WriteData<HtmlNode, Item> writeDataFromAvitela = WriteDataFromAvitela;
            var avitelaItems = avitelaSearch(await Html(httpClient, urlAvitela));
            writeDataFromAvitela(avitelaItems, products);
        }

        private async Task gettingItemsFromPigu(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlPigu = "https://pigu.lt/lt/search?q=" + productName;
            Search<HtmlDocument> piguSearch = PiguSearch;
            WriteData<HtmlNode, Item> writeDataFromPigu = WriteDataFromPigu;
            var piguItems = piguSearch(await Html(httpClient, urlPigu));
            writeDataFromPigu(piguItems, products);
        }

        private async Task gettingItemsFromBigBox(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlBigBox = "https://bigbox.lt/paieska?controller=search&orderby=position&orderway=desc&ssa_submit=&search_query=" + productName;
            Search<HtmlDocument> bigBoxSearch = BigBoxSearch;
            WriteData<HtmlNode, Item> writeDataFromBigBox = WriteDataFromBigBox;
            var bigBoxItems = bigBoxSearch(await Html(httpClient, urlBigBox));
            writeDataFromBigBox(bigBoxItems, products);
        }

        private async Task gettingItemsFromGintarineVaistine(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlGintarineVaistine = "https://www.gintarine.lt/search?adv=false&cid=0&mid=0&vid=0&q=" + productName + "%5D&sid=false&isc=true&orderBy=0";
            Search<HtmlDocument> gintarineVaistineSearch = GintarineVaistineSearch;
            WriteData<HtmlNode, Item> writeDataFromGintarineVaistine = WriteDataFromGintarineVaistine;
            var gintarineVaistineItems = gintarineVaistineSearch(await Html(httpClient, urlGintarineVaistine));
            writeDataFromGintarineVaistine(gintarineVaistineItems, products);
        }

        private async Task gettingItemsFromElektromarkt(string productName, List<Item> products, HttpClient httpClient)
        {
            var urlElektromarkt = "https://elektromarkt.lt/paieska/" + productName;
            Search<HtmlDocument> elektromarktSearch = ElektromarktSearch;
            WriteData<HtmlNode, Item> writeDataFromElektromarkt = WriteDataFromElektromarkt;
            var elektromarktItems = elektromarktSearch(await Html(httpClient, urlElektromarkt));
            writeDataFromElektromarkt(elektromarktItems, products);
        }

        private static async Task<HtmlDocument> Html(HttpClient httpClient, string urlget)
        {
            try
            {
                var url = urlget;
                var html = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                return htmlDocument;
            }
            catch
            {
                return null;
            }
        }
        private static List<HtmlNode> RdeSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                var productsHtml = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("id", "")
                        .Equals("body_div")).ToList();

                var productListItems = productsHtml[0].Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                        .Contains("product_box_div")).ToList();
                return productListItems;
            }

            return null;
        }
        private static List<HtmlNode> AvitelaSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Equals("product-grid active")).ToList();

                    var productListItems = productsHtml[0].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("col-6 col-md-4 col-lg-4")).ToList();
                    return productListItems;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        private static List<HtmlNode> BarboraSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml2 = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Equals("b-page-specific-content")).ToList();

                    var productListItems2 = productsHtml2[0].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("b-product--wrap2 b-product--desktop-grid")).ToList();
                    var productListItemsSoldOut = productsHtml2[0].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("b-product-out-of-stock-backdrop")).ToList();
                    foreach (var unused in productListItemsSoldOut)
                    {
                        SoldOutBarbora++;
                    }

                    return productListItems2;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        private static List<HtmlNode> PiguSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml2 = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("widget-old", "")
                            .Equals("ContentLoader")).ToList();

                    var productListItems2 = productsHtml2[0].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("product-list-item")).ToList();
                    var productListItemsSoldOut = productsHtml2[0].Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("label-soldout")).ToList();
                    foreach (var unused in productListItemsSoldOut)
                    {
                        SoldOut++;
                    }

                    return productListItems2;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private static List<HtmlNode> BigBoxSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Equals("col-lg-9 col-md-8")).ToList();

                    var productListItems = productsHtml[0].Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "")
                            .StartsWith("category-item ajax_block_product col-xs-12 col-sm-6 col-md-4 col-lg-3"))
                        .ToList();
                    return productListItems;

                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
        private static List<HtmlNode> ElektromarktSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml2 = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Equals("manafilters-category-products category-products")).ToList();

                    var productListItems2 = productsHtml2[0].Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("item js-ua-item")).ToList();

                    return productListItems2;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
        private static List<HtmlNode> GintarineVaistineSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Equals("item-grid")).ToList();

                    var productListItems = productsHtml[0].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("item-box")).ToList();
                    return productListItems;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private static void WriteDataFromRde(List<HtmlNode> productListItems, List<Item> products)
        {
            if (productListItems != null)
            {
                foreach (var productListItem in productListItems)
                {

                    var price = productListItem
                        .Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("product_price_wo_discount_listing"))
                        ?.InnerText.Trim();

                    var name = productListItem
                        .Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("product_name"))
                        ?.InnerText.Trim();

                    var link = productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");

                    var productListItems2 = productListItem.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Contains("photo_box")).ToList();
                    foreach (var productListItem2 in productListItems2)
                    {
                        var imgLink = productListItem2.Descendants("img").FirstOrDefault()?.GetAttributeValue("src", "");

                        if (!string.IsNullOrEmpty(price))
                        {
                            price = EliminatingSymbols2(price);
                            var priceAtsarg = price;
                            priceAtsarg = EliminatingSymbols2(priceAtsarg);
                            priceAtsarg = EliminatingEuroSimbol(priceAtsarg);
                            var priceDouble = Convert.ToDouble(priceAtsarg);

                            var singleItem = new Item
                            {
                                Picture = "https://www.rde.lt/" + imgLink,
                                Seller = "Rde",
                                Name = name,
                                PriceDouble = priceDouble,
                                Price = price,
                                Link = "https://www.rde.lt/" + link
                            };
                            products.Add(singleItem);
                        }
                    }
                }
            }
        }

        private static void WriteDataFromAvitela(List<HtmlNode> productListItems, List<Item> products)
        {
            if (productListItems != null)
            {
                foreach (var productListItem in productListItems)
                {

                    var price = productListItem
                        .Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("price"))
                        ?.InnerText.Trim();

                    var name = productListItem
                        .Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("name"))
                        ?.InnerText.Trim();

                    var link = productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");

                    var imgLink = productListItem.Descendants("img").FirstOrDefault()?.GetAttributeValue("data-echo", "");

                    if (!string.IsNullOrEmpty(price))
                    {
                        price = EliminatingSymbols(price);
                        var priceAtsarg = price;
                        priceAtsarg = EliminatingEuroSimbol(priceAtsarg);
                        var priceDouble = Convert.ToDouble(priceAtsarg);
                        if (imgLink != "")
                        {
                            var singleItem = new Item
                            {
                                Picture = imgLink,
                                Seller = "Avitela",
                                Name = name,
                                PriceDouble = priceDouble,
                                Price = price,
                                Link = link
                            };
                            products.Add(singleItem);
                        }
                        else
                        {
                            var singleItem = new Item { Picture = "https://avitela.lt/image/no_image.jpg", Seller = "Avitela", Name = name, PriceDouble = priceDouble, Price = price, Link = link };
                            products.Add(singleItem);
                        }

                    }
                }
            }
        }

        private static void WriteDataFromBarbora(List<HtmlNode> productListItems, List<Item> products)
        {
            if (productListItems != null)
            {
                var countItems = productListItems.Count - SoldOutBarbora;

                foreach (var productListItem in productListItems)
                    if (countItems != 0)
                    {
                        var price = productListItem
                            .Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", "")
                                .Equals("b-product-price-current-number"))
                            ?.InnerText.Trim();

                        var name = productListItem
                            .Descendants("span").FirstOrDefault(node => node.GetAttributeValue("itemprop", "")
                                .Equals("name"))
                            ?.InnerText.Trim();

                        var link = productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");

                        var imgLink = productListItem
                            .Descendants("img").FirstOrDefault(node => node.GetAttributeValue("itemprop", "")
                                .Contains("image"))
                            ?.GetAttributeValue("src", "");

                        if (!string.IsNullOrEmpty(price))
                        {
                            price = EliminatingSymbols(price);
                            var priceTemporary = price;
                            priceTemporary = EliminatingEuroSimbol(priceTemporary);
                            var priceDouble = Convert.ToDouble(priceTemporary);
                            var item1 = new Item
                            {
                                Picture = "https://pagrindinis.barbora.lt/" + imgLink,
                                Seller = "Barbora",
                                Name = name,
                                PriceDouble = priceDouble,
                                Price = price,
                                Link = "https://pagrindinis.barbora.lt/" + link
                            };
                            products.Add(item1);
                        }

                        countItems--;
                    }
            }
        }

        private static void WriteDataFromPigu(List<HtmlNode> productListItems, List<Item> products)
        {
            if (productListItems != null)
            {
                var countItems = productListItems.Count - SoldOut;

                foreach (var productListItem in productListItems)
                    if (countItems != 0)
                    {
                        var price = productListItem
                            .Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", "")
                                .Equals("price notranslate"))
                            ?.InnerText.Trim();
                        var name = productListItem
                            .Descendants("p").FirstOrDefault(node => node.GetAttributeValue("class", "")
                                .Equals("product-name"))
                            ?.InnerText.Trim();

                        var link = "https://pigu.lt/" + productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");

                        var imgLink = productListItem
                            .Descendants("img").FirstOrDefault(node => node.GetAttributeValue("src", "")
                                .Contains("jpg"))
                            ?.GetAttributeValue("src", "");

                        if (!string.IsNullOrEmpty(price))
                        {
                            price = EliminateSpacesPigu(price);
                            var priceAtsarg = price;
                            price = EliminatingEuroSimbol(price);
                            price += "€";
                            priceAtsarg = EliminatingEuroSimbol(priceAtsarg);

                            var priceDouble = Convert.ToDouble(priceAtsarg);
                            var singleItem = new Item
                            {
                                Picture = imgLink,
                                Seller = "Pigu",
                                Name = name,
                                PriceDouble = priceDouble,
                                Price = price,
                                Link = link
                            };
                            products.Add(singleItem);
                            countItems--;
                        }
                    }
            }
        }

        private static void WriteDataFromBigBox(List<HtmlNode> productListItems, List<Item> products)
        {
            if (productListItems != null)
            {
                foreach (var productListItem in productListItems)
                {
                    var price = productListItem
                        .Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("price product-price"))
                        ?.InnerText.Trim();

                    var name = productListItem
                        .Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("product-name"))
                        ?.InnerText.Trim();

                    var link = productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");

                    var imgLink = productListItem
                        .Descendants("img").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Contains("replace-2x img-responsive"))
                        ?.GetAttributeValue("src", "");

                    if (!string.IsNullOrEmpty(price))
                    {
                        price = EliminateSpacesPigu(price);
                        var priceAtsarg = price;
                        price = EliminatingEuroSimbol(price);
                        price += "€";
                        priceAtsarg = EliminatingEuroSimbol(priceAtsarg);
                        var priceDouble = Convert.ToDouble(priceAtsarg);
                        var singleItem = new Item
                        {
                            Picture = imgLink,
                            Seller = "BigBox",
                            Name = name,
                            PriceDouble = priceDouble,
                            Price = price,
                            Link = link
                        };

                        products.Add(singleItem);
                    }
                }
            }
        }

        private static void WriteDataFromElektromarkt(List<HtmlNode> productListItems2, List<Item> products)
        {
            if (productListItems2 != null)
            {
                foreach (var productListItem in productListItems2)
                {

                    var name = productListItem
                        .Descendants("h2").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("product-name"))
                        ?.InnerText.Trim();

                    var price = productListItem
                        .Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("price"))
                        ?.InnerText.Trim();

                    var link = productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");

                    var imgLink = productListItem.Descendants("img").FirstOrDefault()?.GetAttributeValue("src", "");

                    if (!string.IsNullOrEmpty(price))
                    {
                        price = EliminateSpaces(price);
                        var priceAtsarg = price;
                        priceAtsarg = EliminatingEuroSimbol(priceAtsarg);

                        var priceDouble = double.Parse(priceAtsarg);
                        var item1 = new Item
                        {
                            Picture = imgLink,
                            Seller = "Elektromarkt",
                            Name = name,
                            PriceDouble = priceDouble,
                            Price = price,
                            Link = link
                        };
                        products.Add(item1);
                    }
                }
            }
        }

        private static void WriteDataFromGintarineVaistine(List<HtmlNode> productListItems, List<Item> products)
        {
            if (productListItems != null)
            {
                foreach (var productListItem in productListItems)
                {

                    var price = productListItem
                        .Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", "")
                            .Equals("price actual-price"))
                        ?.InnerText.Trim();

                    var name = productListItem.Descendants("input").FirstOrDefault()?.GetAttributeValue("value", "");

                    var link = productListItem.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", "");
                    var imgLink = productListItem.Descendants("img").FirstOrDefault()?.GetAttributeValue("data-lazyloadsrc", "");

                    if (!string.IsNullOrEmpty(price))
                    {
                        var regex = Regex.Match(price ?? string.Empty, @"[0-9]+\,[0-9][0-9]");
                        price = Convert.ToString(regex);
                        var priceDouble = Convert.ToDouble(price);

                        var singleItem = new Item
                        {
                            Picture = imgLink,
                            Seller = "Gintarine vaistine",
                            Name = name,
                            PriceDouble = priceDouble,
                            Price = price + '€',
                            Link = "https://www.gintarine.lt/" + link
                        };
                        products.Add(singleItem);
                    }
                }
            }
        }

        private static string EliminatingEuroSimbol(string priceAtsarg)
        {
            var charsToRemove = new[] { "€" };
            foreach (var c in charsToRemove) priceAtsarg = priceAtsarg.Replace(c, string.Empty);

            return priceAtsarg;
        }

        private static string EliminatingSymbols(string price)
        {
            var index = price.IndexOf("\n", StringComparison.Ordinal);
            if (index > 0) price = price.Substring(0, index);

            var charsToChange = new[] { "." };
            foreach (var c in charsToChange) price = price.Replace(c, ",");
            return price;
        }

        private static string EliminatingSymbols2(string price)
        {
            var index = price.IndexOf("\n", StringComparison.Ordinal);
            if (index > 0) price = price.Substring(0, index);

            var charsToChange = new[] { "." };
            foreach (var c in charsToChange) price = price.Replace(c, ",");
            var charsToChange2 = new[] { "&nbsp;" };
            foreach (var c in charsToChange2) price = price.Replace(c, "");
            var charsToChange3 = new[] { "Kaina: " };
            foreach (var c in charsToChange3) price = price.Replace(c, "");

            return price;
        }
        private static string EliminateSpaces(string price)
        {
            var charsToRemove = new[] { " " };
            foreach (var c in charsToRemove) price = price.Replace(c, string.Empty);
            return price;
        }

        private static string EliminateSpacesPigu(string price)
        {
            var charsToRemove = new[] { " " };
            foreach (var c in charsToRemove) price = price.Replace(c, string.Empty);
            return price;
        }
        private static List<Item> SortAndInsert(List<Item> products)
        {
            products = products.OrderBy(o => o.PriceDouble).ToList();
            return products;
        }
    }
}
