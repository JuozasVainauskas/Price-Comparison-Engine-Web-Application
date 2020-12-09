using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCE_Web.Classes;
using PCE_Web.Tables;

namespace PCE_Web.Models
{
    public interface IReportsManager
    {
        void WriteReports(string email, string report);

        List<Report> ReadReports(string email, int solvedId);

        void DeleteReports(int id);

        bool IsReported(string email);

        void MarkAsSolved(int id);
    }
}
