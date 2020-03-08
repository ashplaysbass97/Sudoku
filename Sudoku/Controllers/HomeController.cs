﻿using Microsoft.AspNetCore.Mvc;
using Sudoku.Models;
using Sudoku.ServiceLayer;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISudokuService _sudokuService = new SudokuService();
        private Grid Grid
        {
            get => HttpContext.Session.GetObject<Grid>("Grid");
            set => HttpContext.Session.SetObject("Grid", value);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewSudoku(string difficulty, int size, string mode)
        {
            Grid = _sudokuService.SetupGrid(size, mode);
            return PartialView("Index", Grid);
        }

        public IActionResult SolveSudoku()
        {
            _sudokuService.SolveSudoku(Grid);
            return PartialView("Index", Grid);
        }
    }
}