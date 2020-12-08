using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using PCE_Web.Models;

namespace PCE_Web.Classes
{
    public class ProductsCache : IProductsCache
    {
        private ObjectCache _cache = MemoryCache.Default;

        public void SetCachedItems(string key, List<Item> products)
        {
            _cache.Set(key, products, DateTimeOffset.Now.AddMinutes(5));
        }
        public IEnumerable<Item> GetCachedItems()
        {
            var productsList = _cache as IEnumerable<Item>;
            return productsList;
        }
        public IEnumerable<Item> GetCachedItems(string key)
        {
            var productsList = _cache[key] as IEnumerable<Item>;
            return productsList;
        }
    }

}
