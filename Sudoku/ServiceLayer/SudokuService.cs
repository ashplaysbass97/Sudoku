using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.ServiceLayer
{
    public class SudokuService : ISudokuService
    {
        public Grid SetupGrid(int size, string mode)
        {
            int[] dimensions = CalculateGridDimensions(size);
            int[,] values = mode == "generate" ? GenerateSudoku(size) : null;

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
                                Value = values?[regionY * dimensions[0] + cellY, regionX * dimensions[1] + cellX]
                            };
                            cells.Add(cell);
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

        public bool SolveSudoku(Grid grid)
        {
            foreach (Cell cell in grid.Cells)
            {
                if (cell.Value == null)
                {
                    for (int value = 1; value <= grid.Size; value++)
                    {
                        if (IsValuePossible(grid, cell, value))
                        {
                            cell.Value = value;
                            if (SolveSudoku(grid))
                            {
                                return true;
                            }
                            cell.Value = null;
                        }
                    }
                    return false;
                }
            }

            // TODO detect an invalid Sudoku
            return true;
        }

        private bool IsValuePossible(Grid grid, Cell cell, int value)
        {
            List<Cell> cellsInHouse = GetCellsInHouse(grid.Size, FindCellRegion(grid, cell), grid.Cells, cell);

            foreach (Cell cellInHouse in cellsInHouse)
            {
                if (cellInHouse.Value == value)
                {
                    return false;
                }
            }
            return true;
        }

        private Region FindCellRegion(Grid grid, Cell cell)
        {
            foreach (Region region in grid.Regions)
            {
                foreach (Cell comparison in region.Cells)
                {
                    if (comparison.Coordinates.Equals(cell.Coordinates))
                    {
                        return region;
                    }
                }
            }

            return null;
        }

        private List<Cell> GetCellsInHouse(int gridSize, Region region, List<Cell> cells, Cell cell)
        {
            List<Cell> cellsInHouse = new List<Cell>();

            for (int i = 0; i < gridSize; i++)
            {
                // Add cells in same row
                if (i != cell.Coordinates.X)
                {
                    cellsInHouse.Add(cells[i + cell.Coordinates.Y * gridSize]);
                }

                // Add cells in same column
                if (i != cell.Coordinates.Y)
                {
                    cellsInHouse.Add(cells[cell.Coordinates.X + i * gridSize]);
                }
            }

            // Add cells in same region
            foreach (Cell cellInHouse in region.Cells)
            {
                if (!cellsInHouse.Contains(cellInHouse) && !cellInHouse.Equals(cell))
                {
                    cellsInHouse.Add(cellInHouse);
                }
            }
            return cellsInHouse;
        }
    }
}