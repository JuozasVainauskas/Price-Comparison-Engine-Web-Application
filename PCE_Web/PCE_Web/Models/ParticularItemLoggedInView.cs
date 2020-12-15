using System.Collections;
using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public class ParticularItemLoggedInView : IEnumerable
    {
        public List<Item> Products { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
