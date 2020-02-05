using System;
using Microsoft.AspNetCore.Mvc;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string NewSudoku(string mode)
        {
            if (mode == "generate")
            {
                return "generate";
            }
            else
            {
                return "solve";
            }
            
        }
    }
}

