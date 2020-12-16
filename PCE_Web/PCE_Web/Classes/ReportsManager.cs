using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class ReportsManager : IReportsManager
    {
        private readonly PceDatabaseContext _pceDatabaseContext;

        public ReportsManager(PceDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void WriteReports(string email, string report)
        {
            if (report != null)
            {
                var newReport = new Reports()
                {
                    Email = email,
                    Comment = report,
                    Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                    Solved = 0
                };
                _pceDatabaseContext.Reports.Add(newReport);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public List<Report> ReadReports(string email, int solvedId)
        {
            var comments = _pceDatabaseContext.Reports.Where(column => column.Email == email && column.Solved == solvedId).Select(column => new Report { Comment = column.Comment, Id = column.ReportsId, Date = column.Date, Email = column.Email }).ToList();
            return comments;
        }

        public void DeleteReports(int id)
        {
            var result = _pceDatabaseContext.Reports.SingleOrDefault(column => column.ReportsId == id);
            if (result != null)
            {
                _pceDatabaseContext.Reports.Remove(result);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public bool IsReported(string email)
        {
            var item = _pceDatabaseContext.Reports.Where(column => column.Email == email).Select(column => new Reports { Email = column.Email, Comment = column.Comment, Solved = column.Solved, Date = column.Date }).ToList();
            if (item.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MarkAsSolved(int id)
        {
            _pceDatabaseContext.Reports.Where(column => column.ReportsId == id && column.Solved == 0).ToList().ForEach(column => column.Solved = 1);
            _pceDatabaseContext.SaveChanges();
        }
    }
}
