using System.Collections.Generic;
using Sudoku.ServiceLayer;

namespace Sudoku.Models
{
    public class Grid
    {
        public Grid(int size, int[] dimensions, List<Cell> cells)
        {
            Size = size;
            Width = dimensions[0];
            Height = dimensions[1];
            Regions = new List<Cell>();
            Cells = cells;
        }

        public int Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Cell> Regions { get; set; }
        public List<Cell> Cells { get; set; }
    }
}