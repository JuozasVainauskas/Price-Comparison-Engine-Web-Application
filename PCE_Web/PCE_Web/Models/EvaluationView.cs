using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using PCE_Web.Classes;
using PCE_Web.Tables;

namespace PCE_Web.Models
{
    public class EvaluationView : IEnumerable
    {
        public List<Comments> Comments { get; set; }
        public int SelectedShopIndex { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
