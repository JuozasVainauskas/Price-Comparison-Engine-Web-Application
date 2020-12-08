using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface IProductsCache
    {
        void SetCachedItems(string key, List<Item> products);

        IEnumerable<Item> GetCachedItems(string key);
    }

}
