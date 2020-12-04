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
        public static int SoldOutBarbora;
        public static int SoldOut;
        public delegate void WriteData<THtmlNode, TItem>(List<THtmlNode> productListItems, List<TItem> products);
        public delegate List<HtmlNode> Search<in THtmlDocument>(THtmlDocument htmlDocument);
        private readonly IHttpClientFactory _httpClient;
        private readonly IDatabaseManager _databaseManager;
        public SearchController(IHttpClientFactory httpClient, IDatabaseManager databaseManager)
        {
            _httpClient = httpClient;
            _databaseManager = databaseManager;
        }

        [AllowAnonymous]
        public ActionResult Suggestions(string productName)
        {
            /*
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
            */
                var products = GetProductsFromAPI(productName);
                _databaseManager.WriteSearchedItems(products, productName);
                var suggestionsView = new SuggestionsView { Products = products };
                return View(suggestionsView);
            //}
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
                var resultList=new List<Item>();
                //var client=new HttpClient();
                var getDataTask = client.GetAsync("https://localhost:44320/api/Products/"+productName).
                      ContinueWith(response =>
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
            catch (Exception )
            {
                Console.WriteLine();
                throw;
            }
        }

        private async Task ReadingItemsAsync(string productName,List<Item> products,HttpClient httpClient)
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
            try
            {
                var urlRde = "https://www.rde.lt/search_result/lt/word/" + productName + "/page/1";
                Search<HtmlDocument> rdeSearch = RdeSearch;
                WriteData<HtmlNode, Item> writeDataFromRde = WriteDataFromRde;
                var rdeItems = rdeSearch(await Html(httpClient, urlRde));
                writeDataFromRde(rdeItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception rde: "+e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }
        private async Task gettingItemsFromBarbora(string productName, List<Item> products, HttpClient httpClient)
        {
            try
            {
                var urlBarbora = "https://pagrindinis.barbora.lt/paieska?q=" + productName;
                Search<HtmlDocument> barboraSearch = BarboraSearch;
                WriteData<HtmlNode, Item> writeDataFromBarbora = WriteDataFromBarbora;
                var barboraItems = barboraSearch(await Html(httpClient, urlBarbora));
                writeDataFromBarbora(barboraItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception Barbora: "+e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }

        private async Task gettingItemsFromAvitela(string productName, List<Item> products, HttpClient httpClient)
        {
            try
            {
                var urlAvitela = "https://avitela.lt/paieska/" + productName;
                Search<HtmlDocument> avitelaSearch = AvitelaSearch;
                WriteData<HtmlNode, Item> writeDataFromAvitela = WriteDataFromAvitela;
                var avitelaItems = avitelaSearch(await Html(httpClient, urlAvitela));
                writeDataFromAvitela(avitelaItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception Avitela: " + e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }

        private async Task gettingItemsFromPigu(string productName, List<Item> products, HttpClient httpClient)
        {
            try
            {
                var urlPigu = "https://pigu.lt/lt/search?q=" + productName;
                Search<HtmlDocument> piguSearch = PiguSearch;
                WriteData<HtmlNode, Item> writeDataFromPigu = WriteDataFromPigu;
                var piguItems = piguSearch(await Html(httpClient, urlPigu));
                writeDataFromPigu(piguItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception Pigu: " + e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }

        private async Task gettingItemsFromBigBox(string productName, List<Item> products, HttpClient httpClient)
        {
            try
            {
                var urlBigBox = "https://bigbox.lt/paieska?controller=search&orderby=position&orderway=desc&ssa_submit=&search_query=" + productName;
                Search<HtmlDocument> bigBoxSearch = BigBoxSearch;
                WriteData<HtmlNode, Item> writeDataFromBigBox = WriteDataFromBigBox;
                var bigBoxItems = bigBoxSearch(await Html(httpClient, urlBigBox));
                writeDataFromBigBox(bigBoxItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception BigBox: " + e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }

        private async Task gettingItemsFromGintarineVaistine(string productName, List<Item> products, HttpClient httpClient)
        {
            try
            {
                var urlGintarineVaistine = "https://www.gintarine.lt/search?adv=false&cid=0&mid=0&vid=0&q=" + productName + "%5D&sid=false&isc=true&orderBy=0";
                Search<HtmlDocument> gintarineVaistineSearch = GintarineVaistineSearch;
                WriteData<HtmlNode, Item> writeDataFromGintarineVaistine = WriteDataFromGintarineVaistine;
                var gintarineVaistineItems = gintarineVaistineSearch(await Html(httpClient, urlGintarineVaistine));
                writeDataFromGintarineVaistine(gintarineVaistineItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception Gitarine Vaistine: " + e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }

        private async Task gettingItemsFromElektromarkt(string productName, List<Item> products, HttpClient httpClient)
        {
            try
            {
                var urlElektromarkt = "https://elektromarkt.lt/paieska/" + productName;
                Search<HtmlDocument> elektromarktSearch = ElektromarktSearch;
                WriteData<HtmlNode, Item> writeDataFromElektromarkt = WriteDataFromElektromarkt;
                var elektromarktItems = elektromarktSearch(await Html(httpClient, urlElektromarkt));
                writeDataFromElektromarkt(elektromarktItems, products);
            }
            catch (Exception e)
            {
                _databaseManager.WriteLoggedExceptions("Exception Elektromarkt: " + e.Message, e.Source, e.StackTrace, DateTime.UtcNow.ToString());
            }
        }

        private async Task<HtmlDocument> Html(HttpClient httpClient, string urlget)
        {
                var url = urlget;
                var html = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                return htmlDocument;
        }
        private static List<HtmlNode> RdeSearch(HtmlDocument htmlDocument)
        {
            if (htmlDocument != null)
            {
                try
                {
                    var productsHtml = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("id", "")
                        .Equals("body_div")).ToList();

                    var productListItems = productsHtml[0].Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Contains("product_box_div")).ToList();
                    return productListItems;
                }
                catch
                {
                    return null;
                }
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
                    var productsHtml = htmlDocument.DocumentNode.Descendants("dyiv")
                        .Where(node => node.GetAttributeValue("clayjyjss", "")
                            .Equals("col-lg-9 col-mdygyyj-8")).ToList();

                    var productListItems = productsHtml[0].Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "")
                            .StartsWith("category-iygygtem ajax_block_product col-xs-12 col-sm-6 col-md-4 col-lg-3"))
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

        private static readonly object Lock=new object();

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
                        var imgLink = productListItem2.Descendants("img").FirstOrDefault()
                            ?.GetAttributeValue("src", "");

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
                            lock (Lock)
                            {
                                AddingToACollection(products, singleItem);
                            }
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
                            lock (Lock)
                            {
                                AddingToACollection(products, singleItem);
                            }
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
                            var singleItem = new Item
                            {
                                Picture = "https://pagrindinis.barbora.lt/" + imgLink, 
                                Seller = "Barbora", 
                                Name = name, 
                                PriceDouble = priceDouble, 
                                Price = price, 
                                Link = "https://pagrindinis.barbora.lt/" + link
                            };
                            lock (Lock)
                            {
                                AddingToACollection(products, singleItem);
                            }
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
                            lock (Lock)
                            {
                                AddingToACollection(products, singleItem);
                            }

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
                        lock (Lock)
                        {
                            AddingToACollection(products, singleItem);
                        }
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
                        var singleItem = new Item
                        {
                            Picture = imgLink,
                            Seller = "Elektromarkt",
                            Name = name,
                            PriceDouble = priceDouble,
                            Price = price,
                            Link = link
                        };
                        lock (Lock)
                        {
                            AddingToACollection(products, singleItem);
                        }

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
                        var regex = Regex.Match(price, @"[0-9]+\,[0-9][0-9]");
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
                        lock (Lock)
                        {
                            AddingToACollection(products,singleItem);
                        }
                    }
                }
            }
        }

        private static void AddingToACollection<T>(ICollection<T> list,T variable)
        {
            list.Add(variable);
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
