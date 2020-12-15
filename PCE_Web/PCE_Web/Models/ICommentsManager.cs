using System.Collections.Generic;
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
