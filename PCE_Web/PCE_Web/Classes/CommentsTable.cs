using System;
using System.Collections.Generic;

namespace PCE_Web.Classes
{
    public partial class CommentsTable
    {
        public int CommentId { get; set; }
        public string Email { get; set; }
        public int ShopId { get; set; }
        public string Date { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
