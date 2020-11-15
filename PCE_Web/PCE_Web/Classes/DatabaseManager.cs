using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class DatabaseManager : IDatabaseManager
    {
        private static bool EmailVerification(string email)
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

        private static bool PasswordVerification(string password)
        {
            var pattern = new Regex(@"(?=(?:.*[a-zA-Z]){3})(?:.*\d)",
                    RegexOptions.Compiled);
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

        /* Admin klasei */
        public void SetRole(string email, string role)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(column => column.Email == email);
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

        public void DeleteAccount(string email)
        {
            using (var context = new PCEDatabaseContext())
            {
                var savedItems = context.SavedItems.Where(column => column.Email == email).ToList();

                foreach (var savedItem in savedItems)
                {
                    context.SavedItems.Remove(savedItem);
                }

                var result = context.UserData.SingleOrDefault(column => column.Email == email);

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

                var comments = context.CommentsTable.Where(column => column.Email == email).ToList();

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

        public void CreateAccount(string email, string password)
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
                var result = context.UserData.SingleOrDefault(column => column.Email == email);
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
        }

        public List<User> ReadUsersList()
        {
            var usersList = new List<User>();

            using (var context = new PCEDatabaseContext())
            {
                var users = context.UserData.Select(column => new UserData() { Email = column.Email, Role = column.Role, PasswordHash = "", PasswordSalt = "", UserId = 0}).ToList();

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
            }

            return usersList;
        }
        /* ------------------------------------------- */

        public static bool CheckIfUserExists(string email)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(column => column.Email == email);
                if (result == null)
                {
                    return false;
                }
            }
            return true;
        }

        public void RegisterUser(string email, string password)
        {
            var passwordSalt = GenerateHash.CreateSalt(10);
            var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(column => column.Email == email);
                if (result == null)
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
        }

        public User LoginUser(string email, string password)
        {
            var user = new User();

            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(column => column.Email == email);

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
            }

            return null;
        }

        public void ChangePassword(string email, string password, string passwordConfirm)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordConfirm))
            {
                //MessageBox.Show("Prašome užpildyti visus laukus.");
            }
            else if (!PasswordVerification(password))
            {
                //MessageBox.Show("Slaptažodyje turi būti bent trys raidės ir vienas skaičius!!!");
            }
            else if (!password.Equals(passwordConfirm))
            {
                //MessageBox.Show("Slaptažodžiai nesutampa.");
            }
            else
            {
                var passwordSalt = GenerateHash.CreateSalt(10);
                var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

                using (var context = new PCEDatabaseContext())
                {
                    var result = context.UserData.SingleOrDefault(column => column.Email == email);
                    if (result != null)
                    {
                        result.PasswordHash = passwordHash;
                        result.PasswordSalt = passwordSalt;
                        context.SaveChanges();
                    }
                    else
                    {
                        //MessageBox.Show("Vartotojas tokiu emailu neegzistuoja arba nebuvo rastas.");
                    }
                }
            }
        }

        public List<Slide> ReadSlidesList()
        {
            List<Slide> slidesList;

            using (var context = new PCEDatabaseContext())
            {
                slidesList = context.ItemsTable.Where(column => column.Price.Length >= 6).Select(column => new Slide() { PageUrl = column.PageUrl, ImgUrl = column.ImgUrl }).ToList();
            }

            return slidesList;
        }

        public void DeleteSavedItem(string email, Item item)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.SavedItems.SingleOrDefault(column =>
                    column.Email == email && column.PageUrl == item.Link && column.ImgUrl == item.Picture &&
                    column.ShopName == item.Seller && column.ItemName == item.Name && column.Price == item.Price);

                if (result != null)
                {
                    context.SavedItems.Remove(result);
                    context.SaveChanges();
                }
            }
        }

        public List<Item> ReadSavedItems(string email)
        {
            var item = new List<Item>();

            using (var context = new PCEDatabaseContext())
            {
                var itemsList = context.SavedItems.Where(column => column.Email == email).Select(column => new Item
                        {Link = column.PageUrl, Picture = column.ImgUrl, Seller = column.ShopName, Name = column.ItemName, Price = column.Price})
                    .ToList();

                foreach (var singleItem in itemsList)
                {
                    item.Add(singleItem);
                }
            }

            return item;
        }

        public void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price,
            string email)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.SavedItems.SingleOrDefault(column =>
                    column.PageUrl == pageUrl && column.ImgUrl == imgUrl && column.ShopName == shopName && column.ItemName == itemName &&
                    column.Price == price && column.Email == email);

                if (result == null)
                {
                    var savedItems = new SavedItems()
                    {
                        PageUrl = pageUrl,
                        ImgUrl = imgUrl,
                        ShopName = shopName,
                        ItemName = itemName,
                        Price = price,
                        Email = email
                    };
                    context.SavedItems.Add(savedItems);
                    context.SaveChanges();
                }
            }
        }
        
        public void WriteLoggedExceptions(string message, string source, string stackTrace)
            {
                using (var context = new PCEDatabaseContext())
                {
                    var result = context.SavedExceptions.SingleOrDefault(column => column.Message == message && column.Source == source && column.StackTrace == stackTrace);

                    if (result == null)
                    {
                        var savedExceptions = new SavedExceptions()
                        {
                            Message = message,
                            Source = source,
                            StackTrace = stackTrace
                        };
                        context.SavedExceptions.Add(savedExceptions);
                        context.SaveChanges();
                    }
                }
            }


        public List<CommentsTable> ReadComments(int index)
        {
            List<CommentsTable> comments;
            using (var context = new PCEDatabaseContext())
            {
                comments = context.CommentsTable
                .Where(column => column.ShopId == index)
                .Select(column => new CommentsTable {CommentId = column.CommentId, Email = column.Email, ShopId = column.ShopId, Date = column.Date, Rating = column.Rating, Comment = column.Comment})
                .ToList();
            }

            return comments;
        }

        public bool IsAlreadyCommented(string email, int shopId)
        {
            List<CommentsTable> item;

            using (var context = new PCEDatabaseContext())
            {
                item = context.CommentsTable.Where(column => column.Email == email && column.ShopId == shopId).Select(column => new CommentsTable
                { CommentId = column.CommentId, Email = column.Email, ShopId = column.ShopId, Date = column.Date, Rating = column.Rating, Comment = column.Comment })
                    .ToList();
            }

            if (item.Count > 0)
            {
                return true;
            }
            else return false;
        }

        public void WriteComments(string email, int shopId, int rating, string comment)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.CommentsTable.SingleOrDefault(column => column.Email == email && column.ShopId == shopId);

                if (result == null)
                {
                    var commentsTable = new CommentsTable()
                    {
                        Email = email,
                        ShopId = shopId,
                        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                        Rating = rating,
                        Comment = comment
                    };
                    context.CommentsTable.Add(commentsTable);
                    context.SaveChanges();
                }
            }
        }

        public ShopRating ReadRatings(string shopName)
        {
            var shopRating = new ShopRating();

            using (var context = new PCEDatabaseContext())
            {
                var result = context.ShopRatingTable.SingleOrDefault(column => column.ShopName == shopName);

                if (result != null)
                {
                    shopRating.VotesNumber = result.VotesNumber;
                    shopRating.VotersNumber = result.VotersNumber;
                }
            }

            return shopRating;
        }

        public void WriteRatings(string shopName, int votesNumber, int votersNumber)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.ShopRatingTable.SingleOrDefault(column => column.ShopName == shopName);

                if (result != null)
                {
                    result.VotersNumber = votersNumber;
                    result.VotesNumber = votesNumber;
                    context.SaveChanges();
                }
            }
        }

        public void WriteSearchedItems(List<Item> items, string productName)
        {
            foreach (var item in items)
            {
                WriteSearchedItem(item.Link, item.Picture, item.Seller, item.Name, item.Price, productName);
            }
        }

        public void WriteSearchedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price, string keyword)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.ItemsTable.SingleOrDefault(column => column.PageUrl == pageUrl && column.ImgUrl == imgUrl && column.ShopName == shopName && column.ItemName == itemName && column.Price == price && column.Keyword == keyword);

                if (result == null)
                {
                    var itemsTable = new ItemsTable
                    {
                        PageUrl = pageUrl,
                        ImgUrl = imgUrl,
                        ShopName = shopName,
                        ItemName = itemName,
                        Price = price,
                        Keyword = keyword
                    };
                    context.ItemsTable.Add(itemsTable);
                    context.SaveChanges();
                }
            }
        }

        public List<Item> ReadSearchedItems(string keyword)
        {
            List<Item> item;

            using (var context = new PCEDatabaseContext())
            {
                item = context.ItemsTable.Where(column => column.Keyword == keyword).Select(column => new Item { Link = column.PageUrl, Picture = column.ImgUrl, Seller = column.ShopName, Name = column.ItemName, Price = column.Price }).ToList();
            }

            return item;
        }
    }
}
