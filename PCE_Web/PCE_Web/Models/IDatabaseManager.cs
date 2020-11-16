﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PCE_Web.Classes;
using PCE_Web.Tables;

namespace PCE_Web.Models
{
    public interface IDatabaseManager
    {
        void SetRole(string email, string role);

        void DeleteAccount(string email);

        void CreateAccount(string email, string password);

        List<User> ReadUsersList();

        void RegisterUser(string email, string password);

        User LoginUser(string email, string password);

        void ChangePassword(string email, string password, string passwordConfirm);

        List<Slide> ReadSlidesList();

        void DeleteSavedItem(string email, Item item);

        List<Item> ReadSavedItems(string email);

        void WriteSavedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price,
            string email);

        List<CommentsTable> ReadComments(int index);

        bool IsAlreadyCommented(string email, int shopId);

        void WriteComments(string email, int shopId, int rating, string comment);

        ShopRating ReadRatings(string shopName);

        void WriteRatings(string shopName, int votesNumber, int votersNumber);

        void WriteLoggedExceptions(string message, string source, string stackTrace, string date);

        void DeleteLoggedExceptions(Exceptions exceptions);

        List<Exceptions> ReadLoggedExceptions();

        void WriteSearchedItems(List<Item> items, string productName);

        void WriteSearchedItem(string pageUrl, string imgUrl, string shopName, string itemName, string price,
            string keyword);

        List<Item> ReadSearchedItems(string keyword);

        void WriteReport(string email, string report);

        bool isReported(string email);
    }
}
