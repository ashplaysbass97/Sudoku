using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sudoku.ServiceLayer
{
    public class SudokuService : ISudokuService
    {
        public int[] CalculateGridDimensions(int size)
        {
            int width = 0, height = 0;
            var sqrt = Math.Sqrt(size);
            if (sqrt % 1 == 0)
                width = height = (int) sqrt;
            else
                for (var i = 1; i < size; i++)
                {
                    double j = size / i;
                    if (j == Math.Floor(j))
                    {
                        width = (int) j;
                        height = i;
                    }

                    if (j <= i) break;
                }

            return new[] {width, height};
        }

        public List<Cell> SetupGrid(int size)
        {
            var grid = new List<Cell>();
            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++)
            {
                var cell = new Cell
                {
                    Coordinates = new Point(x, y)
                };
                grid.Add(cell);
            }

            return grid;
        }
    }
}