using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class AccountManager : IAccountManager
    {
        private readonly PceDatabaseContext _pceDatabaseContext;

        public AccountManager(PceDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        private bool EmailVerification(string email)
        {
            var pattern = new Regex(@"([a-zA-Z0-9._-]*[a-zA-Z0-9][a-zA-Z0-9._-]*)(@gmail.com)$", RegexOptions.Compiled);
            if (string.IsNullOrWhiteSpace(email) || !pattern.IsMatch(email))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool PasswordVerification(string password)
        {
            var pattern = new Regex(@"(?=(?:.*[a-zA-Z]){3})(?:.*\d)", RegexOptions.Compiled);
            if (string.IsNullOrWhiteSpace(password) || !pattern.IsMatch(password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetRole(string email, string role)
        {
            var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);
            if (result != null)
            {
                result.Role = role;
                _pceDatabaseContext.SaveChanges();
            }
        }

        public void DeleteAccount(string email)
        {
            var savedItems = _pceDatabaseContext.SavedItems.Where(column => column.Email == email).ToList();
            foreach (var savedItem in savedItems)
            {
                _pceDatabaseContext.SavedItems.Remove(savedItem);
            }

            var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);
            if (result != null)
            {
                _pceDatabaseContext.UserData.Remove(result);
            }

            var comments = _pceDatabaseContext.Comments.Where(column => column.Email == email).ToList();
            foreach (var comment in comments)
            {
                _pceDatabaseContext.Comments.Remove(comment);
            }

            var reports = _pceDatabaseContext.Reports.Where(column => column.Email == email).ToList();
            foreach (var report in reports)
            {
                _pceDatabaseContext.Reports.Remove(report);
            }
            _pceDatabaseContext.SaveChanges();
        }

        public void CreateAccount(string email, string password)
        {
            var passwordSalt = GenerateHash.CreateSalt(10);
            var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && EmailVerification(email))
            {
                var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);

                if (result == null)
                {
                    var userData = new UserData()
                    {
                        Email = email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Role = "0"
                    };
                    _pceDatabaseContext.UserData.Add(userData);
                    _pceDatabaseContext.SaveChanges();
                }
            }
        }

        public List<User> ReadUsersList()
        {
            var usersList = new List<User>();
            var users = _pceDatabaseContext.UserData.Select(column => new UserData() { Email = column.Email, Role = column.Role, PasswordHash = "", PasswordSalt = "", UserId = 0 }).ToList();

            foreach (var user in users)
            {
                Enum singleTempRole = null;

                if (user.Role == "0")
                {
                    singleTempRole = Role.User;
                }
                else if (user.Role == "1")
                {
                    singleTempRole = Role.Admin;
                }

                usersList.Add(new User()
                {
                    Email = user.Email,
                    Role = singleTempRole
                });

            }
            return usersList;
        }


        public bool CheckIfUserExists(string email)
        {
            var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public void RegisterUser(string email, string password)
        {
            var passwordSalt = GenerateHash.CreateSalt(10);
            var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

            var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);
            if (result == null)
            {
                var userData = new UserData()
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = "0"
                };
                _pceDatabaseContext.UserData.Add(userData);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public User LoginUser(string email, string password)
        {
            var user = new User();
            var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);

            if (result != null)
            {
                var passwordSalt = result.PasswordSalt;
                var passwordHash = result.PasswordHash;
                if (result.Role == "0")
                {
                    user.Role = Role.User;
                }
                else if (result.Role == "1")
                {
                    user.Role = Role.Admin;
                }

                var userEnteredPassword = GenerateHash.GenerateSha256Hash(password, passwordSalt);

                if (passwordHash.Equals(userEnteredPassword))
                {
                    user.Email = email;
                    return user;
                }
            }

            return null;
        }

        public void ChangePassword(string email, string password, string passwordConfirm)
        {
            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(passwordConfirm) && PasswordVerification(password) && password.Equals(passwordConfirm))
            {
                var passwordSalt = GenerateHash.CreateSalt(10);
                var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

                var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);
                if (result != null)
                {
                    result.PasswordHash = passwordHash;
                    result.PasswordSalt = passwordSalt;
                    _pceDatabaseContext.SaveChanges();
                }
            }
        }
    }
}
