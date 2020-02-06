using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku.ServiceLayer
{
    public class SudokuService : ISudokuService
    {
        public List<Cell> SetupGrid()
        {
            List<Cell> grid = new List<Cell>();
            Cell cell = new Cell()
            {
                Coordinates = new Point(0,0)
            };
            grid.Add(cell);
            return grid;
        }
    }
}
