using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCE_Web.Classes;
using PCE_Web.Tables;

namespace PCE_Web.Models
{
    public interface IDatabaseManager
    {
        void SetRole(string email, string role);

        void SetRoleWithDataAdapter(string email, string role);

        void DeleteAccount(string email);

        void CreateAccount(string email, string password);

        List<User> ReadUsersList();

        public bool CheckIfUserExists(string email);

        void RegisterUser(string email, string password);

        User LoginUser(string email, string password);

        void ChangePassword(string email, string password, string passwordConfirm);

        List<Slide> ReadSlidesList();

        void DeleteSavedItem(string email, Item item);

        List<Item> ReadSavedItems(string email);

        void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price,
            string email);

        List<Comments> ReadComments(int index);

        bool IsAlreadyCommented(string email, int shopId);

        void WriteComments(string email, int shopId, int rating, string comment);

        void WriteLoggedExceptions(string message, string source, string stackTrace, string date);

        void DeleteLoggedExceptions(int Id);

        List<Exceptions> ReadLoggedExceptions();

        void WriteReports(string email, string report);

        void WriteReportsWithSql(string email, string report);

        List<Report> ReadReports(string email, int solvedID);

        void DeleteReports(int id);

        void DeleteReportsWithSql(int id);

        bool IsReported(string email);

        void MarkAsSolved(int id);
    }
}
