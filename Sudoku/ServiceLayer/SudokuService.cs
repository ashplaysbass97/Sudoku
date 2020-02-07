using System;
using System.Collections.Generic;
using System.Drawing;
using Sudoku.Models;

namespace Sudoku.ServiceLayer
{
    public class SudokuService : ISudokuService
    {
        public Grid SetupGrid(int size)
        {
            
            List<Cell> cells = new List<Cell>();
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    Cell cell = new Cell
                    {
                        Coordinates = new Point(x, y)
                    };
                    cells.Add(cell);
                }
            }
            Grid grid = new Grid(size, CalculateGridDimensions(size), cells);
            return grid;
        }

        private int[] CalculateGridDimensions(int size)
        {
            int width = 0;
            int height = 0;
            double sqrt = Math.Sqrt(size);
            if (sqrt % 1 == 0)
            {
                width = height = (int)sqrt;
            }
            else
            {
                for (int i = 1; i < size; i++)
                {
                    double j = size / i;
                    if (j == Math.Floor(j))
                    {
                        width = (int)j;
                        height = i;
                    }

                    if (j <= i) break;
                }
            }

            return new[] {width, height};
        }
    }
}