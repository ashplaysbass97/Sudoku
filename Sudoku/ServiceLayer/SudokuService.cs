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

        public Grid GenerateSudoku(Grid grid, string difficulty)
        {
            /*Random r = new Random();
            foreach (int i in Enumerable.Range(0, grid.Cells.Count).OrderBy(x => r.Next()))
            {
                for (int j = 1; j <= grid.Size; j++)
                {
                    if (IsValuePossible(grid, grid.Cells[i], j))
                    {
                        Cell regionCell = FindCellRegion(grid, grid.Cells[i]).Cells
                            .FirstOrDefault((x => x.Coordinates == grid.Cells[i].Coordinates));
                        regionCell.Value = j;

                        grid.Cells[i].Value = j;
                        break;
                    }
                }
            }*/
            return BacktrackingAlgorithm(grid) ? grid : null;
        }

        public Grid UpdateGrid(Grid grid, int?[] sudoku)
        {
            for (int i = 0; i < sudoku.Length; i++)
            {
                grid.Cells[i].Value = sudoku[i];
            }
            return grid;
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
                    Random random = new Random();
                    foreach (int value in Enumerable.Range(1, grid.Size).OrderBy(x => random.Next()))
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