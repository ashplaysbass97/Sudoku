using Microsoft.AspNetCore.Mvc;
using Sudoku.Models;
using Sudoku.ServiceLayer;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISudokuService _sudokuService = new SudokuService();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewSudoku(string difficulty, int size, string mode)
        {
            var grid = new Grid(size, _sudokuService.CalculateGridDimensions(size), _sudokuService.SetupGrid(size));
            return PartialView("_Grid", grid);
        }
    }
}