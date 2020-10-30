using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Models;

namespace PCE_Web.Models
{
    public class ParticularItemView : IEnumerable
    {
        public List<Item> Products { get; set; }
        public string laikinas;

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
