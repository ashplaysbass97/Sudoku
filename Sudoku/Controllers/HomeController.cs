using Microsoft.AspNetCore.Mvc;
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
            return View("NewSudoku");
        }

        public IActionResult NewSudoku(string difficulty, int size, string mode)
        {
            Grid = _sudokuService.SetupGrid(size, mode);
            if (mode == "generate")
            {
                Grid = _sudokuService.GenerateSudoku(Grid, difficulty);
            }
            return PartialView("NewSudoku", Grid);
        }

        public IActionResult SolveSudoku(int?[] sudoku)
        {
            Grid = _sudokuService.SolveSudoku(Grid, sudoku);
            return PartialView("SolveSudoku", Grid);
        }

        public IActionResult SubmitSolution(int?[] sudoku)
        {
            Grid = _sudokuService.SubmitSolution(Grid, sudoku);
            return PartialView("GenerateSudoku", Grid);
        }
    }
}