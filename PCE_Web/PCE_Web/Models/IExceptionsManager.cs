using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCE_Web.Classes;
using PCE_Web.Tables;

namespace PCE_Web.Models
{
    public interface IExceptionsManager
    {
        void WriteLoggedExceptions(string message, string source, string stackTrace, string date);

        void DeleteLoggedExceptions(int id);

        List<Exceptions> ReadLoggedExceptions();
    }
}
