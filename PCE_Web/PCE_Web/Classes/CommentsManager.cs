using System;
using System.Collections.Generic;
using System.Linq;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class CommentsManager : ICommentsManager
    {
        private readonly PceDatabaseContext _pceDatabaseContext;

        public CommentsManager(PceDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public List<Comments> ReadComments(int index)
        {
            var comments = _pceDatabaseContext.Comments.Where(column => column.ShopId == index).Select(column => new Comments { CommentId = column.CommentId, Email = column.Email, ShopId = column.ShopId, Date = column.Date, Rating = column.Rating, Comment = column.Comment }).ToList();
            return comments;
        }

        public bool IsAlreadyCommented(string email, int shopId)
        {
            var item = _pceDatabaseContext.Comments.Where(column => column.Email == email && column.ShopId == shopId).Select(column => new Comments { CommentId = column.CommentId, Email = column.Email, ShopId = column.ShopId, Date = column.Date, Rating = column.Rating, Comment = column.Comment }).ToList();
            if (item.Count > 0)
            {
                return true;
            }
            else return false;
        }

        public void WriteComments(string email, int shopId, int rating, string comment)
        {
            var result = _pceDatabaseContext.Comments.SingleOrDefault(column => column.Email == email && column.ShopId == shopId);
            if (result == null)
            {
                var commentsTable = new Comments()
                {
                    Email = email,
                    ShopId = shopId,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Rating = rating,
                    Comment = comment
                };
                _pceDatabaseContext.Comments.Add(commentsTable);
                _pceDatabaseContext.SaveChanges();
            }
        }
    }
}
