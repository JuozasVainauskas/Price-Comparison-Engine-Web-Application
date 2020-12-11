using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WEBSearchAPI.DTO;
using WEBSearchAPI.Classes;

namespace WEBSearchAPI.Controllers
{
    [Route("api/[controller]/{productName}")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public async Task<IEnumerable<Item>> Get(string productName)
        {
            var httpClient = new HttpClient();
            var products = await SearchingProducts.FetchAlgorithmaAsync(productName, httpClient);
            return products;
        }
    }
}
