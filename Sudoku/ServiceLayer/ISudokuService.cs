using System.Collections.Generic;

namespace Sudoku.ServiceLayer
{
    internal interface ISudokuService
    {
        int[] CalculateGridDimensions(int size);
        List<Cell> SetupGrid(int size);
    }
}