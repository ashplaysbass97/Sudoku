using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudoku.ServiceLayer;

namespace Sudoku.Models
{
    public class Grid
    {
        public List<Cell> Cells { get; set; }

        public Grid()
        {
            Cells = new List<Cell>();
        }
    }
}
