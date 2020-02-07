using System.Collections.Generic;
using System.Drawing;

namespace Sudoku.ServiceLayer
{
    public class Region
    {
        public Region()
        {
            Cells = new List<Cell>();
        }

        public Point Coordinates { get; set; }
        public List<Cell> Cells { get; set; }
    }
}