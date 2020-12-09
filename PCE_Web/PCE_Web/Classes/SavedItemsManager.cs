using System.Collections.Generic;
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

        public void DeleteSavedItem(string email, Item item);

        public List<Item> ReadSavedItems(string email);

        public void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price, string email);
    }
}
