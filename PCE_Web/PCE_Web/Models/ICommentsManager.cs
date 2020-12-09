using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCE_Web.Classes;
using PCE_Web.Tables;

namespace PCE_Web.Models
{
    public interface ICommentsManager
    {
        List<Comments> ReadComments(int index);

        bool IsAlreadyCommented(string email, int shopId);

        void WriteComments(string email, int shopId, int rating, string comment);
    }
}
