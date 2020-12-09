using System.Collections.Generic;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class ReportsManager : IReportsManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public ReportsManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void WriteReports(string email, string report);

        public List<Report> ReadReports(string email, int solvedId);

        public void DeleteReports(int id);

        public bool IsReported(string email);

        public void MarkAsSolved(int id);
    }
}
