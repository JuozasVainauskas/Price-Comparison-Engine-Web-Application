using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface ISuggestionsViewLoggedIn
    {
        public List<Item> ProductsSaved { get; set; }
    }
}
