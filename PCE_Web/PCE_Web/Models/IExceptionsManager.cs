using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface IExceptionsManager
    {
        void WriteLoggedExceptions(string message, string source, string stackTrace, string date);

        void DeleteLoggedExceptions(int id);

        List<Exceptions> ReadLoggedExceptions();
    }
}
