using System.Collections.Generic;
using PCE_Web.Classes;

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
