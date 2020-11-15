using System;
using System.Collections.Generic;

namespace PCE_Web.Tables
{
    public partial class ShopRatingTable
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int VotesNumber { get; set; }
        public int VotersNumber { get; set; }
    }
}
