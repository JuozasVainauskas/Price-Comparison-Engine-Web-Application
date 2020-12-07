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
    public class ParticularItemController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        public ParticularItemController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        [AllowAnonymous]
        public async Task<IActionResult> ParticularItem(string particularItem)
        {
            var httpClient = _httpClient.CreateClient();
            var products = await ParticularItemAlgorithm.ParticularItemAlgorith(particularItem, httpClient);
            var particularItemView = new ParticularItemView { Products = products };
            return View(particularItemView);
        }
    }
}
