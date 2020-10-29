using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class SearchController : Controller
    {
        public async Task<IActionResult> Suggestions(string productName)//Perduodamas produkto pavadinimas
        {
            var httpClient = new HttpClient();
            var products = new List<Item>();
            var urlAvitela = "https://avitela.lt/paieska/" + productName;
            var avitelaItems = AvitelaSearch(await Html(httpClient, urlAvitela));
            WriteDataFromAvitela(avitelaItems, products,productName);
            products = SortAndInsert(products);
            var suggestionsView = new SuggestionsView
            {
                Message = "Artimiausiu metu čia galėsite pamatyti siūlomas prekes.", Products = products
            };
            return View(suggestionsView);
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

        private static void WriteDataFromAvitela(List<HtmlNode> productListItems, List<Item> prices,string product)
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
                        price = PasalinimasTrikdanciuSimboliu(price);
                        var priceAtsarg = price;
                        priceAtsarg = PasalinimasEuroSimbol(priceAtsarg);
                        var pricea = Convert.ToDouble(priceAtsarg);
                        if (imgLink != "")
                        {
                            var singleItem = new Item { Picture = imgLink, Seller = "Avitela", Name = name, PriceDouble = pricea, Price = price, Link = link };
                            prices.Add(singleItem);
                        }
                        else
                        {
                            var singleItem = new Item { Picture = "https://avitela.lt/image/no_image.jpg", Seller = "Avitela", Name = name, PriceDouble = pricea, Price = price, Link = link };
                            prices.Add(singleItem);
                        }

                    }
                }
            }
            else
            {
                var singleItem = new Item { Seller = "Avitela", Name = "tokios prekės " + product + " nėra šioje parduotuvėje" };
                prices.Add(singleItem);
            }
        }

        private static string PasalinimasEuroSimbol(string priceAtsarg)
        {
            var charsToRemove = new[] { "€" };
            foreach (var c in charsToRemove) priceAtsarg = priceAtsarg.Replace(c, string.Empty);

            return priceAtsarg;
        }

        private static string PasalinimasTrikdanciuSimboliu(string price)
        {
            var index = price.IndexOf("\n", StringComparison.Ordinal);
            if (index > 0) price = price.Substring(0, index);

            var charsToChange = new[] { "." };
            foreach (var c in charsToChange) price = price.Replace(c, ",");
            return price;
        }

        private static List<Item> SortAndInsert(List<Item> products)
        {
            products = products.OrderBy(o => o.PriceDouble).ToList();
            return products;
        }
    }
}
