using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PCE_Web.Classes;
using PCE_Web.Models;

namespace PCE_Web.Controllers
{
    public class EvaluationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
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
                    Date = "",
                    Rating = 0,
                    Comment = "Komentarų dar nėra",
                    CommentId = 0
                };
                comments.Add(notExistingComment);
                comments.Add(notExistingComment);
                comments.Add(notExistingComment);
                comments.Add(notExistingComment);
                comments.Add(notExistingComment);
                comments.Add(notExistingComment);
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
