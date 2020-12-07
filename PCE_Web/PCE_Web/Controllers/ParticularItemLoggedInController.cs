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
    public class ParticularItemLoggedInController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        public ParticularItemLoggedInController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> ParticularItemLoggedIn(string particularItem)
        {
            var httpClient = _httpClient.CreateClient();
            var products = await ParticularItemAlgorithm.ParticularItemAlgorith(particularItem, httpClient);
            var particularItemLoggedInView = new ParticularItemLoggedInView { Products = products };
            return View(particularItemLoggedInView);
        }
    }
}