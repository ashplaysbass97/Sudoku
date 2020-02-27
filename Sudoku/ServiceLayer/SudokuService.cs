using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.ServiceLayer
{
    public class SudokuService : ISudokuService
    {
        public Grid SetupGrid(int size)
        {
            int[] dimensions = CalculateGridDimensions(size);
            int[,] values = GenerateSudoku(size);

            List<Region> regions = new List<Region>();
            List<Cell> cells = new List<Cell>();
            for (int regionX = 0; regionX < dimensions[0]; regionX++)
            {
                for (int regionY = 0; regionY < dimensions[1]; regionY++)
                {
                    Region region = new Region
                    {
                        Width = dimensions[1],
                        Height = dimensions[0],
                        Coordinates = new Point(regionX, regionY)
                    };
                    regions.Add(region);

                    for (int cellX = 0; cellX < dimensions[1]; cellX++)
                    {
                        for (int cellY = 0; cellY < dimensions[0]; cellY++)
                        {
                            Cell cell = new Cell
                            {
                                Coordinates = new Point(regionX * dimensions[1] + cellX, regionY * dimensions[0] + cellY),
                                Value = values[regionX * dimensions[1] + cellX, regionY * dimensions[0] + cellY]
                            };
                            cells.Add(cell);

                            cell.Region = region;
                            region.Cells.Add(cell);
                        }
                    }
                }
            }

            // TODO See whether regions and cells can be created efficiently in the correct order
            regions = regions.OrderBy(region => region.Coordinates.Y).ThenBy(region => region.Coordinates.X).ToList();
            cells = cells.OrderBy(cell => cell.Coordinates.Y).ThenBy(cell => cell.Coordinates.X).ToList();

            Grid grid = new Grid(regions, cells);
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
                    double j = (double)size / i;
                    if (j == Math.Floor(j))
                    {
                        width = (int)j;
                        height = i;
                        if (j <= i) break;
                    }
                }
            }

            return new[] {width, height};
        }

        private int[,] GenerateSudoku(int size)
        {
            // TODO Actually generate Sudokus
            if (size == 9)
            {
                return new int[9, 9]
                {
                    {0, 0, 4, 3, 0, 0, 2, 0, 9},
                    {0, 0, 5, 0, 0, 9, 0, 0, 1},
                    {0, 7, 0, 0, 6, 0, 0, 4, 3},
                    {0, 0, 6, 0, 0, 2, 0, 8, 7},
                    {1, 9, 0, 0, 0, 7, 4, 0, 0},
                    {0, 5, 0, 0, 8, 3, 0, 0, 0},
                    {6, 0, 0, 0, 0, 0, 1, 0, 5},
                    {0, 0, 3, 5, 0, 8, 6, 9, 0},
                    {0, 4, 2, 9, 1, 0, 3, 0, 0}
                };
            }
            return new int[size, size];
        }
    }
}