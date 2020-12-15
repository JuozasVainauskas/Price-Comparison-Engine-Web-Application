using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface ISavedItemsManager
    {
        void DeleteSavedItem(string email, Item item);

        List<Item> ReadSavedItems(string email);

        void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price, string email);
    }
}
