using System;
using System.Collections.Generic;

namespace PCE_Web.Models
{
    public partial class UserData
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Role { get; set; }
    }
}
