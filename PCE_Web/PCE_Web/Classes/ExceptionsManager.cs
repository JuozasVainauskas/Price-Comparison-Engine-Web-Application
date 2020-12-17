using System.Collections.Generic;
using System.Linq;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class ExceptionsManager : IExceptionsManager
    {
        private readonly PceDatabaseContext _pceDatabaseContext;

        public ExceptionsManager(PceDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void WriteLoggedExceptions(string message, string source, string stackTrace, string date)
        {
            var result = _pceDatabaseContext.SavedExceptions.SingleOrDefault
                (column => 
                column.Date == date 
                && column.Message == message 
                && column.Source == source 
                && column.StackTrace == stackTrace);

            if (result == null)
            {
                var savedExceptions = new SavedExceptions()
                {
                    Message = message,
                    Source = source,
                    StackTrace = stackTrace,
                    Date = date
                };
                _pceDatabaseContext.SavedExceptions.Add(savedExceptions);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public void DeleteLoggedExceptions(int id)
        {
            var result = _pceDatabaseContext.SavedExceptions.SingleOrDefault(column => column.SavedExceptionId == id);
            if (result != null)
            {
                _pceDatabaseContext.SavedExceptions.Remove(result);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public List<Exceptions> ReadLoggedExceptions()
        {
            var exceptions = _pceDatabaseContext.SavedExceptions.Where(row => row.SavedExceptionId > 0)
                .Select(column => new Exceptions
                {
                    Id = column.SavedExceptionId, 
                    Date = column.Date, 
                    Message = column.Message, 
                    StackTrace = column.StackTrace, 
                    Source = column.Source
                }).ToList();
            return exceptions;
        }
    }
}
