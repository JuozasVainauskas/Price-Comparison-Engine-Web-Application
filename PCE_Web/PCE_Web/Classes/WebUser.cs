using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PCE_Web.Classes
{
    public class WebUser : IdentityUser
    {
        public override string Email { get; set; }
        public override string PasswordHash { get; set; }
    }
}
