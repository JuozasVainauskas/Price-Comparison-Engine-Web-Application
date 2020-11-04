using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PCE_Web.Classes
{
    public static class DatabaseManager
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
            var pattern =
                new Regex(
                    @"(\.*\d+\.*[a-zA-Z]\.*[a-zA-Z]\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*\d+\.*[a-zA-Z]\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*[a-zA-Z]\.*\d+\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*[a-zA-Z]\.*[a-zA-Z]\.*\d+\.*)",
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
        public static void SetRole(string email, string role)
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

        public static void DeleteAccount(string email)
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

        public static void CreateAccount(string email, string password)
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

        public static List<User> ReadUsersList()
        {
            var usersList = new List<User>();

            using (var context = new PCEDatabaseContext())
            {
                var tempEmail = context.UserData.Select(column => column.Email).ToList();
                var tempRole = context.UserData.Select(column => column.Role).ToList();

                for (var i = 0; i < tempRole.Count; i++)
                {
                    Enum singleTempRole = null;

                    if (tempRole[i] == "0")
                    {
                        singleTempRole = Role.User;
                    }
                    else if (tempRole[i] == "1")
                    {
                        singleTempRole = Role.Admin;
                    }

                    usersList.Add(new User()
                    {
                        Email = tempEmail[i],
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
                var result = context.UserData.SingleOrDefault(c => c.Email == email);
                if (result == null)
                {
                    return false;
                }
            }
            return true;
        }

        public static void RegisterUser(string email, string password)
        {
            var passwordSalt = GenerateHash.CreateSalt(10);
            var passwordHash = GenerateHash.GenerateSha256Hash(password, passwordSalt);

            using (var context = new PCEDatabaseContext())
            {
                var result = context.UserData.SingleOrDefault(c => c.Email == email);
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

        public static User LoginUser(string email, string password)
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
                }
            }

            return null;
        }

        public static void ChangePassword(string email, string password, string passwordConfirm)
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
                    var result = context.UserData.SingleOrDefault(b => b.Email == email);
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

        public static List<Slide> ReadSlidesList()
        {
            var slidesList = new List<Slide>();
            using (var context = new PCEDatabaseContext())
            {
                var tempPageUrl = context.ItemsTable.Select(column => column.PageUrl).ToList();
                var tempImgUrl = context.ItemsTable.Select(column => column.ImgUrl).ToList();

                for (var i = 0; i < tempPageUrl.Count; i++)
                {
                    if (tempPageUrl.ElementAt(i) != null && tempImgUrl.ElementAt(i) != null)
                    {
                        slidesList.Add(new Slide()
                        {
                            PageUrl = tempPageUrl[i],
                            ImgUrl = tempImgUrl[i]
                        });
                    }
                }
            }

            return slidesList;
        }

        public static void DeleteSavedItem(string email, Item item)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.SavedItems.SingleOrDefault(b =>
                    b.Email == email && b.PageUrl == item.Link && b.ImgUrl == item.Picture &&
                    b.ShopName == item.Seller && b.ItemName == item.Name && b.Price == item.Price);

                if (result != null)
                {
                    context.SavedItems.Remove(result);
                    context.SaveChanges();
                }
            }
        }

        public static List<Item> ReadSavedItems(string email)
        {
            var item = new List<Item>();

            using (var context = new PCEDatabaseContext())
            {
                var itemsList = context.SavedItems.Where(x => x.Email == email).Select(x => new Item
                        {Link = x.PageUrl, Picture = x.ImgUrl, Seller = x.ShopName, Name = x.ItemName, Price = x.Price})
                    .ToList();

                foreach (var singleItem in itemsList)
                {
                    item.Add(singleItem);
                }
            }

            return item;
        }

        public static void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price,
            string email)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.SavedItems.SingleOrDefault(c =>
                    c.PageUrl == pageUrl && c.ImgUrl == imgUrl && c.ShopName == shopName && c.ItemName == itemName &&
                    c.Price == price && c.Email == email);

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

        public static List<CommentsTable> ReadComments()
        {
            List<CommentsTable> temp;
            using (var context = new PCEDatabaseContext())
            {
                temp = context.CommentsTable.ToList();
            }

            return temp;
        }

        public static void WriteComments(string email, int shopId, int rating, string comment)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.CommentsTable.SingleOrDefault(b => b.Email == email && b.ShopId == shopId);

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

        public static ShopRating ReadRatings(string shopName)
        {
            var shopRating = new ShopRating();

            using (var context = new PCEDatabaseContext())
            {
                var result = context.ShopRatingTable.SingleOrDefault(c => c.ShopName == shopName);

                if (result != null)
                {
                    shopRating.VotesNumber = result.VotesNumber;
                    shopRating.VotersNumber = result.VotersNumber;
                }
            }

            return shopRating;
        }

        public static void WriteRatings(string shopName, int votesNumber, int votersNumber)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.ShopRatingTable.SingleOrDefault(b => b.ShopName == shopName);

                if (result != null)
                {
                    result.VotersNumber = votersNumber;
                    result.VotesNumber = votesNumber;
                    context.SaveChanges();
                }
            }
        }

        public static void WriteSearchedItems(List<Item> items, string productName)
        {
            foreach (var item in items)
            {
                WriteSearchedItem(item.Link, item.Picture, item.Seller, item.Name, item.Price, productName);
            }
        }

        public static void WriteSearchedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price, string keyword)
        {
            using (var context = new PCEDatabaseContext())
            {
                var result = context.ItemsTable.SingleOrDefault(c => c.PageUrl == pageUrl && c.ImgUrl == imgUrl && c.ShopName == shopName && c.ItemName == itemName && c.Price == price && c.Keyword == keyword);

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

        public static List<Item> ReadSearchedItems(string keyword)
        {
             List<Item> item;

            using (var context = new PCEDatabaseContext())
            {
                item = context.ItemsTable.Where(x => x.Keyword == keyword).Select(x => new Item { Link = x.PageUrl, Picture = x.ImgUrl, Seller = x.ShopName, Name = x.ItemName, Price = x.Price }).ToList();
            }
            return item;
        }
    }
}
