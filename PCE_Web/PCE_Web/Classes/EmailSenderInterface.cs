using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Classes
{
    public interface EmailSenderInterface
    {
        void SendEmail(string code, string email);
    }
}