using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Models
{
    public class ShopRatingTable
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int VotesNumber { get; set; }
        public int VotersNumber { get; set; }
    }
}
