﻿using System.Collections;
using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public class SlideshowView : IEnumerable
    {
        public List<Slide> Products { get; set; }
        public List<Item> ProductsSaved { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
