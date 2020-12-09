using System.Collections.Generic;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class ExceptionsManager : IExceptionsManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public ExceptionsManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void WriteLoggedExceptions(string message, string source, string stackTrace, string date);

        public void DeleteLoggedExceptions(int id);

        public List<Exceptions> ReadLoggedExceptions();
    }
}
