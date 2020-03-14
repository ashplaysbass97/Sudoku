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

            List<Cell> cells = new List<Cell>();
            for (int regionX = 0; regionX < dimensions[0]; regionX++)
            {
                for (int regionY = 0; regionY < dimensions[1]; regionY++)
                {
                    for (int cellX = 0; cellX < dimensions[1]; cellX++)
                    {
                        for (int cellY = 0; cellY < dimensions[0]; cellY++)
                        {
                            Cell cell = new Cell
                            {
                                Coordinates = new Point(regionX * dimensions[1] + cellX, regionY * dimensions[0] + cellY),
                                Value = values?[regionY * dimensions[0] + cellY, regionX * dimensions[1] + cellX]
                                Region = new Point(regionX, regionY)
                            };
                            cells.Add(cell);
                        }
                    }
                }
            }

            // TODO See whether regions and cells can be created efficiently in the correct order
            cells = cells.OrderBy(cell => cell.Coordinates.Y).ThenBy(cell => cell.Coordinates.X).ToList();

            Grid grid = new Grid
            {
                Size = size,
                RegionWidth = dimensions[1],
                RegionHeight = dimensions[0],
                Cells = cells
            };
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

        public Grid UpdateGrid(Grid grid, int?[] sudoku)
        {
            for (int i = 0; i < sudoku.Length; i++)
            {
                grid.Cells[i].Value = sudoku[i];
            }
            return grid;
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

        public Grid SolveSudoku(Grid grid)
        {
            return BacktrackingAlgorithm(grid) ? grid : null;
        }

        private bool BacktrackingAlgorithm(Grid grid)
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
                            if (BacktrackingAlgorithm(grid))
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
            List<Cell> cellsInHouse = GetCellsInHouse(grid.Size, grid.Cells, cell);

            foreach (Cell cellInHouse in cellsInHouse)
            {
                if (cellInHouse.Value == value)
                {
                    return false;
                }
            }
            return true;
        }

        private List<Cell> GetCellsInHouse(int gridSize, List<Cell> cells, Cell cell)
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
            foreach (Cell cellInHouse in cells.Where(x => x.Region == cell.Region))
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