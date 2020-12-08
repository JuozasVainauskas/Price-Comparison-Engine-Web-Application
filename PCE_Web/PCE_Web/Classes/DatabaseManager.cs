using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using PCE_Web.Models;
using PCE_Web.Tables;

//startup su interface, multiple interface, interface grupės, inicijuoti tik reikalingus
//group atvaizduoti prekių skaičių pagal kategoriją, pagal shop
//aggregate-count
//-join

namespace PCE_Web.Classes
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public DatabaseManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        private static bool EmailVerification(string email)
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

        private static bool PasswordVerification(string password)
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

        /* Admin klasei */
        public void SetRole(string email, string role)
        {
            var result = _pceDatabaseContext.UserData.SingleOrDefault(column => column.Email == email);
            if (result != null)
            {
                result.Role = role;
                _pceDatabaseContext.SaveChanges();
            }
        }

        public void SetRoleWithDataAdapter(string email, string role)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            var sqlDataAdapter = new SqlDataAdapter("SELECT Role, Email FROM UserData;", sqlConnection);
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                var update = new SqlCommand
                {
                    Connection = sqlConnection,
                    CommandType = CommandType.Text,
                    CommandText = "UPDATE UserData SET Role = @Role WHERE Email = @Email;"
                };

                update.Parameters.Add(new SqlParameter("@Role", SqlDbType.NVarChar, int.MaxValue, "Role"));
                update.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, int.MaxValue, "Email"));

                sqlDataAdapter.UpdateCommand = update;

                var dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "UserData");

                DataTable dataTable = dataSet.Tables["UserData"];
                dataTable.Rows[0]["Role"] = role;
                dataTable.Rows[0]["Email"] = email;

                sqlDataAdapter.Update(dataSet, "UserData");
            }
            catch (Exception ex)
            {
                WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
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

            if(!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && EmailVerification(email))
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
            var users = _pceDatabaseContext.UserData.Select(column => new UserData() { Email = column.Email, Role = column.Role, PasswordHash = "", PasswordSalt = "", UserId = 0}).ToList();

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
        /* ------------------------------------------- */

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

        public void DeleteSavedItem(string email, Item item)
        {
            var result = _pceDatabaseContext.SavedItems.SingleOrDefault(column =>
                column.Email == email && column.PageUrl == item.Link && column.ImgUrl == item.Picture &&
                column.ShopName == item.Seller && column.ItemName == item.Name && column.PriceWithSymbol == item.Price);

            if (result != null)
            {
                _pceDatabaseContext.SavedItems.Remove(result);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public List<Item> ReadSavedItems(string email)
        {
            var items = _pceDatabaseContext.SavedItems.Where(column => column.Email == email).Select(column => new Item
                    {Link = column.PageUrl, Picture = column.ImgUrl, Seller = column.ShopName, Name = column.ItemName, Price = column.PriceWithSymbol }).ToList();

            return items;
        }

        public void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price, string email)
        {
            
            var result = _pceDatabaseContext.SavedItems.SingleOrDefault(column =>
                column.PageUrl == pageUrl && column.ImgUrl == imgUrl && column.ShopName == shopName && column.ItemName == itemName &&
                column.PriceWithSymbol == price && column.Email == email);

            if (result == null)
            {
                var savedItems = new SavedItems()
                {
                    PageUrl = pageUrl,
                    ImgUrl = imgUrl,
                    ShopName = shopName,
                    ItemName = itemName,
                    PriceWithSymbol = price,
                    Email = email
                };
                _pceDatabaseContext.SavedItems.Add(savedItems);
                _pceDatabaseContext.SaveChanges();
            }
        }
        
        public void WriteLoggedExceptions(string message, string source, string stackTrace, string date)
        {
            var result = _pceDatabaseContext.SavedExceptions.SingleOrDefault(column => column.Date == date && column.Message == message && column.Source == source && column.StackTrace == stackTrace);

            if (result == null)
            {
                var savedExceptions = new SavedExceptions()
                {
                    Message = message,
                    Source = source,
                    StackTrace = stackTrace,
                    Date = date
                };
                _pceDatabaseContext.SavedExceptions.Add(savedExceptions);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public List<Exceptions> ReadLoggedExceptions()
        {
            var exceptions = _pceDatabaseContext.SavedExceptions.Where(row => row.SavedExceptionId > 0)
                .Select(column => new Exceptions { Id = column.SavedExceptionId, Date = column.Date, Message = column.Message, StackTrace = column.StackTrace, Source = column.Source }).ToList();
            return exceptions;
        }

        public void DeleteLoggedExceptions(int id)
        {
            var result = _pceDatabaseContext.SavedExceptions.SingleOrDefault(column => column.SavedExceptionId == id );
            if (result != null)
            {
                _pceDatabaseContext.SavedExceptions.Remove(result);
                _pceDatabaseContext.SaveChanges();
            }
        }

        public List<Comments> ReadComments(int index)
        {
            var comments = _pceDatabaseContext.Comments.Where(column => column.ShopId == index).Select(column => new Comments {CommentId = column.CommentId, Email = column.Email, ShopId = column.ShopId, Date = column.Date, Rating = column.Rating, Comment = column.Comment}).ToList();
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

        public void WriteReports(string email, string report)
        {
            if (report != null)
            {
                var newReport = new Reports()
                {
                    Email = email,
                    Comment = report,
                    Date = DateTime.UtcNow.ToString(),
                    Solved = 0
                };
                _pceDatabaseContext.Reports.Add(newReport);
                _pceDatabaseContext.SaveChanges();
            }
        }
        public void WriteReportsWithSql(string email, string report)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                var sqlCommand = new SqlCommand("INSERT INTO Reports(Email, Comment, Date, Solved) VALUES (@Email, @Comment, @Date, @Solved)", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@Email", email);
                sqlCommand.Parameters.AddWithValue("@Comment", report);
                sqlCommand.Parameters.AddWithValue("@Date", DateTime.UtcNow.ToString());
                sqlCommand.Parameters.AddWithValue("@Solved", 0);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public List<Report> ReadReports(string email, int solvedID)
        {
            var comments = _pceDatabaseContext.Reports.Where(column => column.Email == email && column.Solved == solvedID).Select(column => new Report { Comment = column.Comment, ID = column.ReportsId, Date = column.Date, Email = column.Email }).ToList();
            return comments;
        }

        public void DeleteReports(int id)
        {
            var result = _pceDatabaseContext.Reports.SingleOrDefault(column => column.ReportsId == id);
            if (result != null)
            {
                _pceDatabaseContext.Reports.Remove(result);
                _pceDatabaseContext.SaveChanges();
            }
        }
        public void DeleteReportsWithSql(int id)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                var sqlCommand = new SqlCommand("DELETE FROM Reports WHERE ReportsId = @Id;", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@Id", id);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool IsReported(string email)
        {
            var item = _pceDatabaseContext.Reports.Where(column => column.Email == email).Select(column => new Reports { Email = column.Email, Comment = column.Comment, Solved = column.Solved, Date = column.Date}).ToList();
            if (item.Count > 0)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }

        public void MarkAsSolved(int id)
        {
            _pceDatabaseContext.Reports.Where(column => column.ReportsId == id && column.Solved == 0).ToList().ForEach(column => column.Solved = 1);
            _pceDatabaseContext.SaveChanges();
        }
    }
}
