using System;
using System.Collections.Generic;

namespace PCE_Web.Models
{
    public partial class SavedItems
    {
        public int SavedItemId { get; set; }
        public string Email { get; set; }
        public string PageUrl { get; set; }
        public string ImgUrl { get; set; }
        public string ShopName { get; set; }
        public string ItemName { get; set; }
        public string Price { get; set; }
    }
}
