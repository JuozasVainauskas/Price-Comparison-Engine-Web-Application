using System.Collections.Generic;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class AccountManager : IAccountManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public AccountManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void SetRole(string email, string role);

        public void DeleteAccount(string email);

        public void CreateAccount(string email, string password);

        public List<User> ReadUsersList();

        public bool CheckIfUserExists(string email);

        public void RegisterUser(string email, string password);

        public User LoginUser(string email, string password);

        public void ChangePassword(string email, string password, string passwordConfirm);
    }
}
