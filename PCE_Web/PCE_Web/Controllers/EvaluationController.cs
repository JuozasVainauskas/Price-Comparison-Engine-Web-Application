﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    [Authorize]
    public class EvaluationController : Controller
    {

        public IActionResult Evaluate(int index)
        {
            if (DatabaseManager.ReadComments(index).Any())
            {
                var comments = DatabaseManager.ReadComments(index);
                var evaluationView = new EvaluationView
                {
                    Comments = comments
                };
                return View(evaluationView);
            }
            else
            {
                var comments = new List<CommentsTable>();
                var notExistingComment = new CommentsTable
                {
                    Email = "info@smartshop.lt",
                    ShopId = 0,
                    Date = DateTime.Now.ToString("MM/dd/yyyy"),
                    Rating = 0,
                    Comment = "Komentarų dar nėra",
                    CommentId = 0
                };
                comments.Add(notExistingComment);

                var evaluationView = new EvaluationView
                {
                    Comments = comments
                };
                return View(evaluationView);
            }
        }
    }
}
