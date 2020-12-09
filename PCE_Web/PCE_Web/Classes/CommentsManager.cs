using System.Collections.Generic;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class CommentsManager : ICommentsManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public CommentsManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public List<Comments> ReadComments(int index);

        public bool IsAlreadyCommented(string email, int shopId);

        public void WriteComments(string email, int shopId, int rating, string comment);
    }
}
