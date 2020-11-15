using System;
using System.Collections.Generic;

namespace PCE_Web.Tables
{
    public partial class ItemsTable
    {
        public int ItemId { get; set; }
        public string PageUrl { get; set; }
        public string ImgUrl { get; set; }
        public string ShopName { get; set; }
        public string ItemName { get; set; }
        public string Price { get; set; }
        public string Keyword { get; set; }
    }
}
