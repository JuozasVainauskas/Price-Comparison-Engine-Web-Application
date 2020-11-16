using System;
using System.Collections.Generic;

namespace PCE_Web.Tables
{
    public partial class SavedExceptions
    {
        public int SavedExceptionId { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
    }
}
