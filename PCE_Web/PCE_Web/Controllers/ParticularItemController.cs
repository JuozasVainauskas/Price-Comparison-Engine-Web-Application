using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class ParticularItemController : Controller
    {
        public static int SoldOutBarbora;
        public static int SoldOut;
        public static string[] Divided;
        private static readonly string[] ItemsToSkip = { "Šaldytuvas", "Išmanusis", "telefonas", "Kompiuteris", "mobilusis", "apsauginis", "stiklas" };
        public async Task<IActionResult> ParticularItem(string particularItem)
        {
            Divided = particularItem.Split();
            var httpClient = new HttpClient();
            var products = new List<Item>();
            var urlRde = "https://www.rde.lt/search_result/lt/word/" + particularItem + "/page/1";
            var urlAvitela = "https://avitela.lt/paieska/" + particularItem;
            var urlBarbora = "https://pagrindinis.barbora.lt/paieska?q=" + particularItem;
            var urlPigu = "https://pigu.lt/lt/search?q=" + particularItem;
            var urlBigBox = "https://bigbox.lt/paieska?controller=search&orderby=position&orderway=desc&ssa_submit=&search_query=" + particularItem;
            var urlElektromarkt = "https://www.elektromarkt.lt/lt/catalogsearch/result/?order=price&dir=desc&q=" + particularItem;
            var urlGintarineVaistine = "https://www.gintarine.lt/search?adv=false&cid=0&mid=0&vid=0&q=" + particularItem + "%5D&sid=false&isc=true&orderBy=0";

            var rdeItems = RdeSearch(await Html(httpClient, urlRde));
            WriteDataFromRde(rdeItems, products);
            var barboraItems = BarboraSearch(await Html(httpClient, urlBarbora));
            WriteDataFromBarbora(barboraItems, products);
            var avitelaItems = AvitelaSearch(await Html(httpClient, urlAvitela));
            WriteDataFromAvitela(avitelaItems, products);
            var piguItems = PiguSearch(await Html(httpClient, urlPigu));
            WriteDataFromPigu(piguItems, products);
            var bigBoxItem = BigBoxSearch(await Html(httpClient, urlBigBox));
            WriteDataFromBigBox(bigBoxItem, products);
            var gintarineVaistineItems = GintarineVaistineSearch(await Html(httpClient, urlGintarineVaistine));
            WriteDataFromGintarineVaistine(gintarineVaistineItems, products);
            var elektromarktItems = ElektromarktSearch(await Html(httpClient, urlElektromarkt));
            WriteDataFromElektromarkt(elektromarktItems, products);
            products = SortAndInsert(products);

            var particularItemView = new ParticularItemView
            {
                Products = products
            };
            return View(particularItemView);
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

        private static List<HtmlNode> BarboraSearch(HtmlDocument htmlDocument2)
        {
            try
            {
                var productsHtml2 = htmlDocument2.DocumentNode.Descendants("div")
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

        private static List<HtmlNode> PiguSearch(HtmlDocument htmlDocument2)
        {

            if (htmlDocument2 != null)
            {
                var productsHtml2 = htmlDocument2.DocumentNode.Descendants("div")
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

            return null;
        }

        private static List<HtmlNode> BigBoxSearch(HtmlDocument htmlDocument)
        {
            try
            {
                var productsHtml = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                        .Equals("col-lg-9 col-md-8")).ToList();

                var productListItems = productsHtml[0].Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                        .StartsWith("category-item ajax_block_product col-xs-12 col-sm-6 col-md-4 col-lg-3")).ToList();
                return productListItems;

            }
            catch
            {
                return null;
            }
        }
        private static List<HtmlNode> ElektromarktSearch(HtmlDocument htmlDocument2)
        {

            try
            {
                var productsHtml2 = htmlDocument2.DocumentNode.Descendants("div")
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
        private static List<HtmlNode> GintarineVaistineSearch(HtmlDocument htmlDocument)
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

        private static void WriteDataFromRde(List<HtmlNode> productListItems, List<Item> prices)
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

                        if (price != "")
                        {
                            price = EliminatingSymbols2(price);
                            var priceBackUp = price;
                            priceBackUp = EliminatingSymbols2(priceBackUp);
                            priceBackUp = EliminatingEuroSimbol(priceBackUp);
                            var pricea = Convert.ToDouble(priceBackUp);
                            if (name != null)
                            {
                                var pavArray = name.Split();
                                var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                                if (numberOfSameWords >= (Divided.Length / 2) + 1)
                                {
                                    var singleItem = new Item
                                    {
                                        Picture = "https://www.rde.lt/" + imgLink,
                                        Seller = "Rde",
                                        Name = name,
                                        PriceDouble = pricea,
                                        Price = price,
                                        Link = "https://www.rde.lt/" + link
                                    };
                                    prices.Add(singleItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void WriteDataFromAvitela(List<HtmlNode> productListItems, List<Item> prices)
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

                    if (price != "")
                    {
                        price = EliminatingSymbols(price);
                        var priceAtsarg = price;
                        priceAtsarg = EliminatingEuroSimbol(priceAtsarg);
                        var pricea = Convert.ToDouble(priceAtsarg);
                        if (name != null)
                        {
                            var pavArray = name.Split();
                            var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                            if (numberOfSameWords >= (Divided.Length / 2) + 1)
                            {
                                var singleItem = new Item
                                {
                                    Picture = imgLink,
                                    Seller = "Avitela", 
                                    Name = name, 
                                    PriceDouble = pricea, 
                                    Price = price, 
                                    Link = link
                                };
                                prices.Add(singleItem);
                            }
                        }

                    }
                }
            }
        }

        private static void WriteDataFromBarbora(List<HtmlNode> productListItems, List<Item> prices)
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

                        if (price != "")
                        {
                            price = EliminatingSymbols(price);
                            var priceTemporary = price;
                            priceTemporary = EliminatingEuroSimbol(priceTemporary);
                            var pricea = Convert.ToDouble(priceTemporary);
                            if (name != null)
                            {
                                var pavArray = name.Split();
                                var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                                if (numberOfSameWords >= (Divided.Length / 2) + 1)
                                {
                                    var singleItem = new Item
                                    {
                                        Picture = "https://pagrindinis.barbora.lt/" + imgLink,
                                        Seller = "Barbora",
                                        Name = name,
                                        PriceDouble = pricea,
                                        Price = price,
                                        Link = "https://pagrindinis.barbora.lt/" + link
                                    };
                                    prices.Add(singleItem);
                                }
                            }
                        }

                        countItems--;
                    }
            }
        }

        private static void WriteDataFromPigu(List<HtmlNode> productListItems, List<Item> prices)
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

                        if (price != null)
                        {
                            price = EliminateSpacesPigu(price);
                            var priceAtsarg = price;
                            price = EliminatingEuroSimbol(price);
                            price = price + "€";
                            priceAtsarg = EliminatingEuroSimbol(priceAtsarg);

                            var pricea = Convert.ToDouble(priceAtsarg);
                            if (name != null)
                            {
                                var pavArray = name.Split();
                                var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                                if (numberOfSameWords >= (Divided.Length / 2) + 1)
                                {
                                    var singleItem = new Item
                                    {
                                        Picture = imgLink,
                                        Seller = "Pigu",
                                        Name = name,
                                        PriceDouble = pricea,
                                        Price = price,
                                        Link = link
                                    };
                                    prices.Add(singleItem);
                                    countItems--;
                                }
                            }
                        }
                    }
            }
        }

        private static void WriteDataFromBigBox(List<HtmlNode> productListItems, List<Item> prices)
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

                    if (price != "")
                    {
                        price = EliminateSpacesPigu(price);
                        var priceAtsarg = price;
                        price = EliminatingEuroSimbol(price);
                        price = price + "€";
                        priceAtsarg = EliminatingEuroSimbol(priceAtsarg);
                        var pricea = Convert.ToDouble(priceAtsarg);
                        if (name != null)
                        {
                            var pavArray = name.Split();
                            var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                            if (numberOfSameWords >= (Divided.Length / 2) + 1)
                            {
                                var singleItem = new Item
                                {
                                    Picture = imgLink,
                                    Seller = "BigBox",
                                    Name = name,
                                    PriceDouble = pricea,
                                    Price = price,
                                    Link = link
                                };
                                prices.Add(singleItem);
                            }
                        }
                    }
                }
            }
        }

        private static void WriteDataFromElektromarkt(List<HtmlNode> productListItems2, List<Item> prices)
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

                    price = EliminateSpaces(price);
                    var priceAtsarg = price;
                    priceAtsarg = EliminatingEuroSimbol(priceAtsarg);

                    var pricea = double.Parse(priceAtsarg);
                    if (name != null)
                    {
                        var pavArray = name.Split();
                        var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                        if (numberOfSameWords >= (Divided.Length / 2) + 1)
                        {
                            var singleItem = new Item
                            {
                                Picture = imgLink,
                                Seller = "Elektromarkt",
                                Name = name,
                                PriceDouble = pricea,
                                Price = price,
                                Link = link
                            };
                            prices.Add(singleItem);
                        }
                    }

                }
            }
        }

        private static void WriteDataFromGintarineVaistine(List<HtmlNode> productListItems, List<Item> prices)
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

                    if (price != "")
                    {
                        var regex = Regex.Match(price ?? string.Empty, @"[0-9]+\,[0-9][0-9]");
                        price = Convert.ToString(regex);
                        var pricea = Convert.ToDouble(price);

                        if (name != null)
                        {
                            var pavArray = name.Split();
                            var numberOfSameWords = AlgorithmHowManyWordsAreTheSame(pavArray);
                            if (numberOfSameWords >= (Divided.Length / 2) + 1)
                            {
                                var singleItem = new Item
                                {
                                    Picture = imgLink,
                                    Seller = "Gintarine vaistine",
                                    Name = name,
                                    PriceDouble = pricea,
                                    Price = price + '€',
                                    Link = "https://www.gintarine.lt/" + link
                                };
                                prices.Add(singleItem);
                            }
                        }
                    }
                }
            }
        }

        private static int AlgorithmHowManyWordsAreTheSame(string[] pavArray)
        {
            var numberOfSameWords = 0;
            foreach (var t in pavArray)
            {
                foreach (var t1 in Divided) 
                {
                        if (t.Equals(t1, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var acceptTheWord = 1;
                            foreach (var t2 in ItemsToSkip)
                            {
                                if (t.Equals(t2, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    acceptTheWord = 0;
                                }
                            }

                            if (acceptTheWord == 1)
                            {
                                numberOfSameWords++;
                            }
                        }
                }
            }

            return numberOfSameWords;
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
