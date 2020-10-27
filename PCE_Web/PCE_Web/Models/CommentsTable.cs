using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Models
{
    public class CommentsTable
    {
        public int CommentId { get; set; }
        public string Email { get; set; }
        public int ShopId { get; set; }
        public string Date { get; set; }
        public int ServiceRating { get; set; }
        public int ProductsQualityRating { get; set; }
        public int DeliveryRating { get; set; }
        public string Comment { get; set; }
    }
}
