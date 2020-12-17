using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using PCE_Web.Models;

namespace PCE_Web.Classes
{
    public class ProductsCache : IProductsCache
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public void SetCachedItems(string key, List<Item> products)
        {
            _cache.Set(key, products, DateTimeOffset.Now.AddMinutes(5));
        }

        public List<IEnumerable<Item>> GetCachedItems()
        {
            var productsList = new List<IEnumerable<Item>>();
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach(var cacheKey in cacheKeys)
            {
                productsList.Add(_cache[cacheKey] as IEnumerable<Item>);
            }
            
            return productsList;
        }
        public IEnumerable<Item> GetCachedItems(string key)
        {
            var productsList = _cache[key] as IEnumerable<Item>;
            return productsList;
        }
    }

}
