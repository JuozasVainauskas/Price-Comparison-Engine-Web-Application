using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PCE_Web.Classes
{
    public class DatabaseManagementClass
    {
        private void SetRole(string email, string role)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(b => b.Email == email);
                if (result != null)
                {
                    result.Role = role;
                    context.SaveChanges();
                    //UpdateStatistics();
                }
                else
                {
                    //MessageBox.Show("Vartotojas tokiu emailu neegzistuoja arba nebuvo rastas.");
                }
            }
        }

        private void DeleteAccount(string email)
        {
            using (var context = new PCEDatabaseContext())
            {
                var savedItems = context.SavedItems.Where(c => c.Email == email).ToList();

                foreach (var savedItem in savedItems)
                {
                    context.SavedItems.Remove(savedItem);
                }

                var result = context.UserData.SingleOrDefault(c => c.Email == email);

                if (result != null)
                {
                    context.UserData.Remove(result);
                    //UpdateStatistics();
                    //MessageBox.Show("Vartotojas " + email + " buvo ištrintas iš duomenų bazės!");
                }
                else
                {
                    //MessageBox.Show("Vartotojas tokiu emailu neegzistuoja arba nebuvo rastas.");
                }

                var comments = context.CommentsTable.Where(c => c.Email == email).ToList();

                foreach (var comment in comments)
                {
                    context.CommentsTable.Remove(comment);
                }

                context.SaveChanges();
            }

            //if (email == LoginWindow.Email)
            //{
            //    LoginWindow.Email = "";
            //    LoginWindow.UserRole = Classes.Role.User;
            //    var mainWindow = new MainWindow();
            //    mainWindow.Show();
            //    _mainWindowLoggedIn.Close();
            //    this.Close();
            //}
        }
        private bool EmailVerification(string email)
        {
            var pattern = new Regex(@"([a-zA-Z0-9._-]*[a-zA-Z0-9][a-zA-Z0-9._-]*)(@gmail.com)$", RegexOptions.Compiled);
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            else if (!pattern.IsMatch(email))
            {
                return false;
            }
            else return true;
        }
        private bool PasswordVerification(string password)
        {
            var pattern = new Regex(@"(\.*\d+\.*[a-zA-Z]\.*[a-zA-Z]\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*\d+\.*[a-zA-Z]\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*[a-zA-Z]\.*\d+\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*[a-zA-Z]\.*[a-zA-Z]\.*\d+\.*)", RegexOptions.Compiled);
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            else if (!pattern.IsMatch(password))
            {
                return false;
            }
            else return true;
        }

        private void CreateAccount(string email, string password)
        {
            var passwordSalt = GenerateHash.CreateSalt(10);
            var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                //MessageBox.Show("Prašome užpildyti visus laukus.");
            }
            else if (!EmailVerification(email))
            {
                //MessageBox.Show("Neteisingai suformatuotas el. paštas!");
            }
            else
            {
                var context = new PCEDatabaseContext();
                var result = context.UserData.SingleOrDefault(c => c.Email == email);
                if (result != null)
                {
                    //MessageBox.Show("Toks email jau panaudotas. Pabandykite kitą.");
                }
                else
                {
                    var userData = new UserData()
                    {
                        Email = email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Role = "0"
                    };
                    context.UserData.Add(userData);
                    context.SaveChanges();
                    //UpdateStatistics();
                    //MessageBox.Show("Vartotojas sekmingai sukurtas!");
                }
            }
        }

        private void RegisterUser(string email, string password)
        {
            var passwordSalt = GenerateHash.CreateSalt(10);
            var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

            var context = new PCEDatabaseContext();
            var result = context.UserData.SingleOrDefault(c => c.Email == email);
            if (result != null)
            {
                //MessageBox.Show("Toks email jau panaudotas. Pabandykite kitą.");
            }
            else
            {
                var userData = new UserData()
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = "0"
                };
                context.UserData.Add(userData);
                context.SaveChanges();
            }
        }

        private User LoginUser(string email, string password)
        {
            var user = new User();

            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(c => c.Email == email);

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
                    else
                    {
                        //MessageBox.Show("Blogai įvestas slaptažodis!");
                    }
                }
                else
                {
                    //MessageBox.Show("Toks email nerastas arba įvestas blogai!");
                }
            }

            user.Role = null;
            user.Email = null;
            return user;
        }
    }
}
