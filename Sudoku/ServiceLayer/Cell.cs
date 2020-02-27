using System.Drawing;

namespace Sudoku.ServiceLayer
{
    public class Cell
    {
        public Point Coordinates { get; set; }
        public Region Region { get; set; }
        public int Value { get; set; }
    }
}