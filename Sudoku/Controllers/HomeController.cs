using System;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Models;
using Sudoku.ServiceLayer;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {
        private ISudokuService sudokuService = new SudokuService();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewSudoku(string difficulty, int width, int height, string mode)
        {


            Grid grid = new Grid() { Cells = sudokuService.SetupGrid() };
            return PartialView("_Grid", grid);

        }
    }
}

