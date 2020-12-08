using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace PCE_Web.Classes
{
    public class ProductsCache
    {
        private static ObjectCache _cache = System.Runtime.Caching.MemoryCache.Default;

        public static void SetCachedItems(string key, List<Item> products)
        {
            _cache.Set(key, products, DateTimeOffset.Now.AddMinutes(5));
        }
        public static IEnumerable<Item> GetCachedItems(string key)
        {
            var productsList = _cache[key] as IEnumerable<Item>;
            return productsList;
        }
    }

}
