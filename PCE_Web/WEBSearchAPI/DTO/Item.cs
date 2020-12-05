using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEBSearchAPI.DTO
{
    public class Item
    {
        public string Picture { get; set; }
        public string Seller { get; set; }
        public double PriceDouble { get; set; }
        public string Price { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
