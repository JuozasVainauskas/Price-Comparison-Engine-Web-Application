using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface ISuggestionsView
    {
        public List<Item> Products { get; set; }
    }
}
