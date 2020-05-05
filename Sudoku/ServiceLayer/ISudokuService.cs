using Sudoku.Models;

namespace Sudoku.ServiceLayer
{
    internal interface ISudokuService
    {
        Grid SetupGrid(int size, string mode);
        Grid GenerateSudoku(Grid grid, string difficulty);
        Grid SolveSudoku(Grid grid, int?[] sudoku);
        Grid SubmitSolution(Grid grid, int?[] sudoku);
    }
}