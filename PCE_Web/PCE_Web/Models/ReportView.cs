using System.Collections;
using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public class ReportView : IEnumerable
    {
        public List<Report> UnsolvedComments { get; set; }
        public List<Report> SolvedComments { get; set; }

        public List<Report>AllComments { get; set; }

        public string Role;
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
