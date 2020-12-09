using System.Collections.Generic;
using System.Linq;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class SavedItemsManager : ISavedItemsManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public SavedItemsManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void DeleteSavedItem(string email, Item item)
        {
            var result = _pceDatabaseContext.SavedItems.SingleOrDefault(column =>
                column.Email == email && column.PageUrl == item.Link && column.ImgUrl == item.Picture &&
                column.ShopName == item.Seller && column.ItemName == item.Name && column.PriceWithSymbol == item.Price);

            if (result != null)
            {
                _pceDatabaseContext.SavedItems.Remove(result);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public List<Item> ReadSavedItems(string email)
        {
            var items = _pceDatabaseContext.SavedItems.Where(column => column.Email == email).Select(column => new Item
            { Link = column.PageUrl, Picture = column.ImgUrl, Seller = column.ShopName, Name = column.ItemName, Price = column.PriceWithSymbol }).ToList();

            return items;
        }

        public void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price, string email)
        {

            var result = _pceDatabaseContext.SavedItems.SingleOrDefault(column =>
                column.PageUrl == pageUrl && column.ImgUrl == imgUrl && column.ShopName == shopName && column.ItemName == itemName &&
                column.PriceWithSymbol == price && column.Email == email);

            if (result == null)
            {
                var savedItems = new SavedItems()
                {
                    PageUrl = pageUrl,
                    ImgUrl = imgUrl,
                    ShopName = shopName,
                    ItemName = itemName,
                    PriceWithSymbol = price,
                    Email = email
                };
                _pceDatabaseContext.SavedItems.Add(savedItems);
                _pceDatabaseContext.SaveChanges();
            }
        }
    }
}
