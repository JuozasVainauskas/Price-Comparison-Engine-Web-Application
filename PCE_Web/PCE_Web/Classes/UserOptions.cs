using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Classes
{
    public class UserOptions
    {
        public const string EmailSend = "EmailSend";
        
        public string SecretMail { get; set; }
        
        public string SecretPassword { get; set; }
    }
}
