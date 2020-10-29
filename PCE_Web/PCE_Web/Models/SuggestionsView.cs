using System.Collections;
using System.Collections.Generic;

namespace PCE_Web.Models
{
    public class SuggestionsView : IEnumerable
    {
        public string Message { get; set; }
        public List<Item> Products { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
