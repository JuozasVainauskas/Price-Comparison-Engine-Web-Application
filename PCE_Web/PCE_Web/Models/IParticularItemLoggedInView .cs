using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface IParticularItemLoggedInView
    {
        public List<Item> Products { get; set; }
    }
}
