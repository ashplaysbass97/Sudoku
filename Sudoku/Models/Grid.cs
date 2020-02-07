using System.Collections.Generic;
using Sudoku.ServiceLayer;

namespace Sudoku.Models
{
    public class Grid
    {
        public int Size { get; set; }
        public List<Region> Regions { get; set; }
        public List<Cell> Cells { get; set; }

        public Grid(List<Region> regions, List<Cell> cells)
        {
            Size = regions.Count;
            Regions = regions;
            Cells = cells;
        }
    }
}
