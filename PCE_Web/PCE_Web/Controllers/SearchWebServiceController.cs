using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WEBSearchAPI.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PCE_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchWebServiceController : ControllerBase
    {
        // GET: api/<SearchWebServiceController>
        [HttpGet]
        public List<Item> GetData()
        {
            List<Item> products = new List<Item>();
            for (int i = 0; i < 10; i++)
            {
                var band = new Item
                {
                    Picture = "a",
                    Seller = "a",
                    PriceDouble = 2,
                    Price = "a",
                    Name = "a",
                    Link = "aaa"
                };
                products.Add(band);
            }
            return products;
        }
    }
}
