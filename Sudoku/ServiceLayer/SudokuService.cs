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
            int[] dimensions = CalculateGridDimensions(size);

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
                                Coordinates = new Point(regionX * dimensions[1] + cellX, regionY * dimensions[0] + cellY)
                            };
                            cells.Add(cell);

                            cell.Region = region;
                            region.Cells.Add(cell);
                        }
                    }
                }
            }

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
    }
}