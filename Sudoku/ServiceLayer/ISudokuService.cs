using System.Collections.Generic;
using Sudoku.Models;

namespace Sudoku.ServiceLayer
{
    internal interface ISudokuService
    {
        Grid SetupGrid(int size, string mode);
        Grid UpdateGrid(Grid grid, int?[] sudoku);
        Grid SolveSudoku(Grid grid);
    }
}