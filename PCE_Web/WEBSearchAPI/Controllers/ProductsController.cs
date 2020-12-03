using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WEBSearchAPI.DTO;

namespace WEBSearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        List<Item> products=new List<Item>();
        public ProductsController()
        {
            GetSampleProducts();
        }

        public IEnumerable<Item> Get()
        {
            return products;
        }

        private void GetSampleProducts()
        {
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
        }
    }
}
