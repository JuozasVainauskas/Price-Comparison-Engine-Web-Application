﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PCE_Web.Controllers
{
    public class EvaluationSuccessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Success()
        {
            return View();
        }
    }
}
