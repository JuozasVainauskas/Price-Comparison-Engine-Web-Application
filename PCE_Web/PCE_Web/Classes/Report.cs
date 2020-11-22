using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Classes
{
    public class Report
    {
        public int ID
        {
            get;
            set;
        }
        public string Comment
        {
            get;
            set;
        }
        public string Date
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public int Solved
        {
            get;
            set;
        }
    }
}
