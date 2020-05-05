using System.Collections.Generic;
using Sudoku.ServiceLayer;

namespace Sudoku.Models
{
    public class Grid
    {
        public int Size { get; set; }
        public int RegionWidth { get; set; }
        public int RegionHeight { get; set; }
        public List<Cell> Cells { get; set; }
        public bool Solved { get; set; }
    }
}