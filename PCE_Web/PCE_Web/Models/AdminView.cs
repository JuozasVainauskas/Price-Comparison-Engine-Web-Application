﻿using System.Collections;
using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public class AdminView : IEnumerable
    {
        public List<User> Users { get; set; }

        public List<User> ReportedUsers { get; set; }
        public List<Exceptions> Exceptions { get; set; }
        public string Role;

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
