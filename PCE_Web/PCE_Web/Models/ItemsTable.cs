using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Models
{
    public class ItemsTable
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
