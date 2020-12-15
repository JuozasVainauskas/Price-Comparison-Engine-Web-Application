using System.Collections.Generic;
using PCE_Web.Classes;

namespace PCE_Web.Models
{
    public interface IAccountManager
    {
        void SetRole(string email, string role);

        void DeleteAccount(string email);

        void CreateAccount(string email, string password);

        List<User> ReadUsersList();

        bool CheckIfUserExists(string email);

        void RegisterUser(string email, string password);

        User LoginUser(string email, string password);

        void ChangePassword(string email, string password, string passwordConfirm);
    }
}
